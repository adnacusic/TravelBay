"""AIAgentSlike: Google Custom Search (image search) finds a picture for every destination that has no
image yet. The query is the first AI keyword (or name + city when there are none); the URL and the
source site are stored in DestinationImages with IsAiGenerated = 1."""

import time

import pymssql
import requests

from ..config import Settings
from ..runs import RunReporter
from . import FatalAgentError

SEARCH_URL = "https://www.googleapis.com/customsearch/v1"
RESULTS_PER_QUERY = 3
REQUEST_TIMEOUT_SECONDS = 20

# Same limits as the DestinationImages columns.
MAX_IMAGE_URL_LENGTH = 500
MAX_SOURCE_LENGTH = 200
DEFAULT_SOURCE = "Google Custom Search"

# Google answers 400/403 for a wrong key or engine id and for an exhausted daily quota:
# every further call would fail the same way, so the run stops.
FATAL_STATUS_CODES = {400, 401, 403}


def _pending_destinations(conn: pymssql.Connection) -> list[dict]:
    with conn.cursor(as_dict=True) as cursor:
        cursor.execute(
            """SELECT d.Id, d.Name, d.Keywords, c.Name AS City
               FROM Destinations d
               JOIN Cities c ON c.Id = d.CityId
               WHERE d.IsDeleted = 0
                 AND NOT EXISTS (SELECT 1 FROM DestinationImages i WHERE i.DestinationId = d.Id)
               ORDER BY d.Id"""
        )
        return cursor.fetchall()


def search_query(destination: dict) -> str:
    keywords = [k.strip() for k in (destination["Keywords"] or "").split(",") if k.strip()]
    return keywords[0] if keywords else f"{destination['Name']} {destination['City']}"


def _search_image(settings: Settings, query: str) -> tuple[str, str] | None:
    response = requests.get(
        SEARCH_URL,
        params={
            "key": settings.google_api_key,
            "cx": settings.google_cx_id,
            "q": query,
            "searchType": "image",
            "num": RESULTS_PER_QUERY,
            "safe": "active",
        },
        timeout=REQUEST_TIMEOUT_SECONDS,
    )
    if response.status_code in FATAL_STATUS_CODES:
        raise FatalAgentError(
            f"Google Custom Search je odbio zahtjev (HTTP {response.status_code}) — "
            "provjerite GOOGLE_API_KEY, GOOGLE_CX_ID i dnevnu kvotu."
        )
    response.raise_for_status()

    for item in response.json().get("items", []):
        link = item.get("link", "")
        if link.startswith(("http://", "https://")) and len(link) <= MAX_IMAGE_URL_LENGTH:
            source = (item.get("displayLink") or DEFAULT_SOURCE)[:MAX_SOURCE_LENGTH]
            return link, source
    return None


def _save(conn: pymssql.Connection, destination_id: int, image_url: str, source: str) -> bool:
    """False when an image was added meanwhile (e.g. by an admin), so nothing is inserted."""
    with conn.cursor() as cursor:
        cursor.execute(
            """INSERT INTO DestinationImages (DestinationId, ImageUrl, Source, OrderIndex, IsAiGenerated, CreatedAt)
               SELECT %d, %s, %s, 0, 1, SYSUTCDATETIME()
               WHERE NOT EXISTS (SELECT 1 FROM DestinationImages WHERE DestinationId = %d)""",
            (destination_id, image_url, source, destination_id),
        )
        inserted = cursor.rowcount == 1
    conn.commit()
    return inserted


def run(conn: pymssql.Connection, settings: Settings, reporter: RunReporter) -> None:
    if not settings.google_api_key or not settings.google_cx_id:
        raise FatalAgentError("GOOGLE_API_KEY ili GOOGLE_CX_ID nije postavljen u .env — AIAgentSlike ne može raditi.")

    destinations = _pending_destinations(conn)
    reporter.start(len(destinations))
    reporter.log(
        f"AIAgentSlike: {len(destinations)} destinacija bez slike "
        f"(pauza {settings.image_search_delay_seconds:g} s između poziva)."
    )

    for index, destination in enumerate(destinations):
        name = destination["Name"]
        query = search_query(destination)
        try:
            found = _search_image(settings, query)
        except requests.RequestException as ex:
            reporter.log(f"Greška za {name}: pretraga '{query}' nije uspjela ({ex.__class__.__name__}).")
            reporter.item_done(success=False)
            found = None
        else:
            if found is None:
                reporter.log(f"{name}: nema rezultata za '{query}'.")
                reporter.item_done(success=False)
            elif _save(conn, destination["Id"], *found):
                reporter.log(f"{name}: slika sa {found[1]} (upit '{query}').")
                reporter.item_done(success=True)
            else:
                reporter.log(f"{name}: u međuvremenu je dodana slika, preskočeno.")
                reporter.item_done(success=True)

        if index < len(destinations) - 1:
            time.sleep(settings.image_search_delay_seconds)
