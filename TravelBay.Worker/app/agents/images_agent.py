"""AIAgentSlike: finds a free picture on Wikimedia Commons for every destination that has no image
yet, and stores its URL and source in DestinationImages with IsAiGenerated = 1.

The query is the first AI keyword (name + place). If Commons has nothing for it, shorter queries
follow: name + city, then the name alone. The agent originally used Google Custom Search, but Google
closed the Custom Search JSON API to new projects (403 PERMISSION_DENIED "This project does not have
the access to Custom Search JSON API"); Commons needs no key or billing and its images are freely
licensed. The logic is the same, only the image source changed."""

import re
import time
from urllib.parse import parse_qsl, urlencode, urlsplit, urlunsplit

import pymssql
import requests

from ..config import Settings
from ..runs import RunReporter
from . import FatalAgentError

SEARCH_URL = "https://commons.wikimedia.org/w/api.php"
# Wikimedia's API policy asks every client to identify itself with a descriptive User-Agent.
USER_AGENT = "TravelBay-Worker/1.0 (RS2 student project; https://github.com/adnacusic/TravelBay)"
RESULTS_PER_QUERY = 5
THUMBNAIL_WIDTH = 1280
REQUEST_TIMEOUT_SECONDS = 20

# Photos only (no GIF flags/maps, no SVG drawings).
ACCEPTED_MIME_TYPES = {"image/jpeg", "image/png", "image/webp"}

# Same limits as the DestinationImages columns.
MAX_IMAGE_URL_LENGTH = 500
MAX_SOURCE_LENGTH = 200
SOURCE_NAME = "Wikimedia Commons"

_TAGS = re.compile(r"<[^>]+>")
_TRACKING_PREFIX = "utm_"


def _without_tracking(url: str) -> str:
    """Commons adds utm_* parameters to unscaled image URLs; the image itself does not need them."""
    parts = urlsplit(url)
    query = [(k, v) for k, v in parse_qsl(parts.query) if not k.startswith(_TRACKING_PREFIX)]
    return urlunsplit(parts._replace(query=urlencode(query)))


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


def search_queries(destination: dict) -> list[str]:
    """First AI keyword first, then shorter fallbacks; duplicates removed, order kept."""
    keywords = [k.strip() for k in (destination["Keywords"] or "").split(",") if k.strip()]
    candidates = [
        keywords[0] if keywords else "",
        f"{destination['Name']} {destination['City']}",
        destination["Name"],
    ]
    queries: list[str] = []
    for query in candidates:
        if query and query.lower() not in (q.lower() for q in queries):
            queries.append(query)
    return queries


def _plain(metadata: dict, field: str) -> str:
    value = metadata.get(field, {}).get("value", "")
    return " ".join(_TAGS.sub("", str(value)).split())


def _source(metadata: dict) -> str:
    """e.g. "Wikimedia Commons · CC BY-SA 3.0 · Pudelek (Marcin Szala)" — license and author for attribution."""
    parts = [SOURCE_NAME, _plain(metadata, "LicenseShortName"), _plain(metadata, "Artist")]
    return " · ".join(p for p in parts if p)[:MAX_SOURCE_LENGTH]


def _search_image(query: str) -> tuple[str, str] | None:
    response = requests.get(
        SEARCH_URL,
        params={
            "action": "query",
            "format": "json",
            "formatversion": 2,
            "generator": "search",
            "gsrsearch": f"{query} filetype:bitmap",
            "gsrnamespace": 6,  # File: pages
            "gsrlimit": RESULTS_PER_QUERY,
            "prop": "imageinfo",
            "iiprop": "url|mime|extmetadata",
            "iiurlwidth": THUMBNAIL_WIDTH,
            "iiextmetadatafilter": "LicenseShortName|Artist",
        },
        headers={"User-Agent": USER_AGENT},
        timeout=REQUEST_TIMEOUT_SECONDS,
    )
    if response.status_code == 403:
        raise FatalAgentError("Wikimedia Commons je odbio zahtjev (HTTP 403) — provjerite User-Agent workera.")
    response.raise_for_status()

    pages = response.json().get("query", {}).get("pages", [])
    for page in sorted(pages, key=lambda p: p.get("index", 0)):
        info = (page.get("imageinfo") or [{}])[0]
        url = _without_tracking(info.get("thumburl") or info.get("url", ""))
        if info.get("mime") in ACCEPTED_MIME_TYPES and url.startswith("https://") and len(url) <= MAX_IMAGE_URL_LENGTH:
            return url, _source(info.get("extmetadata", {}))
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


def _find(settings: Settings, queries: list[str]) -> tuple[str, tuple[str, str]] | None:
    """The first query with a usable image, paced like every other API call."""
    for attempt, query in enumerate(queries):
        if attempt > 0:
            time.sleep(settings.image_search_delay_seconds)
        found = _search_image(query)
        if found is not None:
            return query, found
    return None


def _process(conn: pymssql.Connection, settings: Settings, destination: dict, reporter: RunReporter) -> None:
    name = destination["Name"]
    queries = search_queries(destination)
    try:
        result = _find(settings, queries)
    except requests.RequestException as ex:
        reporter.log(f"Greška za {name}: pretraga nije uspjela ({ex.__class__.__name__}).")
        reporter.item_done(success=False)
        return

    if result is None:
        reporter.log(f"{name}: nema slike na Wikimedia Commons (upiti: {' | '.join(queries)}).")
        reporter.item_done(success=False)
        return

    query, (image_url, source) = result
    if _save(conn, destination["Id"], image_url, source):
        reporter.log(f"{name}: slika za '{query}' — {source}.")
    else:
        reporter.log(f"{name}: u međuvremenu je dodana slika, preskočeno.")
    reporter.item_done(success=True)


def run(conn: pymssql.Connection, settings: Settings, reporter: RunReporter) -> None:
    destinations = _pending_destinations(conn)
    reporter.start(len(destinations))
    reporter.log(
        f"AIAgentSlike: {len(destinations)} destinacija bez slike, izvor {SOURCE_NAME} "
        f"(pauza {settings.image_search_delay_seconds:g} s između poziva)."
    )

    for index, destination in enumerate(destinations):
        if index > 0:
            time.sleep(settings.image_search_delay_seconds)
        _process(conn, settings, destination, reporter)
