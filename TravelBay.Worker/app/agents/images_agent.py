"""AIAgentSlike: finds a free picture on Wikimedia Commons for every destination that has no image
yet, and stores its URL and source in DestinationImages with IsAiGenerated = 1.

Several queries are tried, most specific first (name; name without type words such as "Vodopadi";
name + city + country; the AI name keyword; destination or city + a descriptive keyword; city +
country). Each result is judged against the destination
(image_relevance.py): files that match neither the name nor the city are dropped as obvious
misses, a GOOD match beats an APPROXIMATE one (only the place matches), and approximate images
are listed at the end of the run log so the admin can replace them by hand. A photo already used
for another destination is not reused.

The agent originally used Google Custom Search, but Google closed the Custom Search JSON API to new
projects (403 PERMISSION_DENIED "This project does not have the access to Custom Search JSON API");
Commons needs no key or billing and its images are freely licensed. The logic is the same, only the
image source changed."""

import re
import time
from dataclasses import dataclass
from urllib.parse import parse_qsl, urlencode, urlsplit, urlunsplit

import pymssql
import requests

from ..config import Settings
from ..runs import RunReporter
from . import FatalAgentError
from .image_relevance import DestinationTerms, Match, Relevance, is_type_word, judge, normalize

SEARCH_URL = "https://commons.wikimedia.org/w/api.php"
# Wikimedia's API policy asks every client to identify itself with a descriptive User-Agent.
USER_AGENT = "TravelBay-Worker/1.0 (RS2 student project; https://github.com/adnacusic/TravelBay)"
RESULTS_PER_QUERY = 10
# How many descriptive keywords are tried as "<destination or city> <keyword>" queries.
CONCEPT_QUERIES = 2
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
_NAME_SEPARATORS = re.compile(r"[\s(),.—-]+")


def _without_tracking(url: str) -> str:
    """Commons adds utm_* parameters to unscaled image URLs; the image itself does not need them."""
    parts = urlsplit(url)
    query = [(k, v) for k, v in parse_qsl(parts.query) if not k.startswith(_TRACKING_PREFIX)]
    return urlunsplit(parts._replace(query=urlencode(query)))


def _pending_destinations(conn: pymssql.Connection) -> list[dict]:
    with conn.cursor(as_dict=True) as cursor:
        cursor.execute(
            """SELECT d.Id, d.Name, d.Keywords, c.Name AS City, co.Name AS Country
               FROM Destinations d
               JOIN Cities c ON c.Id = d.CityId
               JOIN Countries co ON co.Id = c.CountryId
               WHERE d.IsDeleted = 0
                 AND NOT EXISTS (SELECT 1 FROM DestinationImages i WHERE i.DestinationId = d.Id)
               ORDER BY d.Id"""
        )
        return cursor.fetchall()


def _used_image_urls(conn: pymssql.Connection) -> set[str]:
    """Images already shown for some destination; the same photo is not given to a second one."""
    with conn.cursor() as cursor:
        cursor.execute("SELECT ImageUrl FROM DestinationImages")
        return {row[0] for row in cursor.fetchall()}


def keywords_of(destination: dict) -> list[str]:
    return [k.strip() for k in (destination["Keywords"] or "").split(",") if k.strip()]


def search_queries(destination: dict) -> list[str]:
    """Most specific first: the name as written (Commons titles are often in the local language), the
    name without its type words, name + city + country, the AI name keyword, the destination (or its
    city) with what it is (e.g. "Trebinje wine cellar"), and the place alone as the last resort."""
    name, city, country = destination["Name"], destination["City"], destination["Country"]
    keywords = keywords_of(destination)
    # "Vodopadi Kravica" -> "Kravica": most Commons titles are English, the Bosnian type word hurts.
    identity = " ".join(w for w in _NAME_SEPARATORS.split(name) if w and not is_type_word(w))
    subject = identity if identity and normalize(identity) != normalize(city) else city
    candidates = [name, identity, f"{name} {city} {country}", keywords[0] if keywords else ""]
    candidates += [f"{subject} {keyword}" for keyword in keywords[1:1 + CONCEPT_QUERIES]]
    candidates.append(f"{city} {country}")

    queries: list[str] = []
    for query in candidates:
        query = " ".join(query.split())
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


@dataclass(frozen=True)
class Candidate:
    url: str
    source: str
    title: str
    query: str
    relevance: Relevance


def _search_images(query: str) -> list[dict]:
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
            "iiextmetadatafilter": "LicenseShortName|Artist|ObjectName|ImageDescription|Categories",
        },
        headers={"User-Agent": USER_AGENT},
        timeout=REQUEST_TIMEOUT_SECONDS,
    )
    if response.status_code == 403:
        raise FatalAgentError("Wikimedia Commons je odbio zahtjev (HTTP 403) — provjerite User-Agent workera.")
    response.raise_for_status()

    pages = response.json().get("query", {}).get("pages", [])
    return sorted(pages, key=lambda p: p.get("index", 0))


def _candidates(pages: list[dict], terms: DestinationTerms, query: str, used: set[str]) -> list[Candidate]:
    result: list[Candidate] = []
    for page in pages:
        info = (page.get("imageinfo") or [{}])[0]
        url = _without_tracking(info.get("thumburl") or info.get("url", ""))
        if (info.get("mime") not in ACCEPTED_MIME_TYPES or not url.startswith("https://")
                or len(url) > MAX_IMAGE_URL_LENGTH or url in used):
            continue
        metadata = info.get("extmetadata", {})
        title = page.get("title", "")
        details = " ".join(_plain(metadata, f) for f in ("ObjectName", "ImageDescription", "Categories"))
        relevance = judge(terms, title, details)
        if relevance.match != Match.MISS:
            result.append(Candidate(url, _source(metadata), title, query, relevance))
    return result


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


def _is_best_possible(candidate: Candidate, terms: DestinationTerms) -> bool:
    """A good match named in the file title that also shows what the destination is: stop searching."""
    relevance = candidate.relevance
    return relevance.match == Match.GOOD and relevance.in_title and (relevance.concept_hits > 0 or not terms.concepts)


def _find(settings: Settings, destination: dict, used: set[str]) -> Candidate | None:
    """Tries the queries in order and keeps the best candidate: GOOD before APPROXIMATE, then a
    match in the file title, then by score."""
    terms = DestinationTerms.of(
        destination["Name"], destination["City"], destination["Country"], keywords_of(destination))
    best: Candidate | None = None
    for attempt, query in enumerate(search_queries(destination)):
        if attempt > 0:
            time.sleep(settings.image_search_delay_seconds)
        for candidate in _candidates(_search_images(query), terms, query, used):
            if best is None or _rank(candidate) > _rank(best):
                best = candidate
        if best is not None and _is_best_possible(best, terms):
            break
    return best


def _rank(candidate: Candidate) -> tuple[bool, bool, float]:
    relevance = candidate.relevance
    return relevance.match == Match.GOOD, relevance.in_title, relevance.score


def _process(conn: pymssql.Connection, settings: Settings, destination: dict, used: set[str],
             reporter: RunReporter, approximate: list[str]) -> None:
    name = destination["Name"]
    try:
        best = _find(settings, destination, used)
    except requests.RequestException as ex:
        reporter.log(f"Greška za {name}: pretraga nije uspjela ({ex.__class__.__name__}).")
        reporter.item_done(success=False)
        return

    if best is None:
        reporter.log(f"{name}: Wikimedia Commons nema sliku koja odgovara nazivu ili mjestu — dodajte je ručno.")
        reporter.item_done(success=False)
        return

    if not _save(conn, destination["Id"], best.url, best.source):
        reporter.log(f"{name}: u međuvremenu je dodana slika, preskočeno.")
        reporter.item_done(success=True)
        return

    used.add(best.url)
    label = "DOBRA" if best.relevance.match == Match.GOOD else "PRIBLIŽNA"
    if best.relevance.match == Match.APPROXIMATE:
        approximate.append(name)
    reporter.log(f"{name}: [{label}] {best.title.removeprefix('File:')} (upit '{best.query}') — {best.source}.")
    reporter.item_done(success=True)


def run(conn: pymssql.Connection, settings: Settings, reporter: RunReporter) -> None:
    destinations = _pending_destinations(conn)
    reporter.start(len(destinations))
    reporter.log(
        f"AIAgentSlike: {len(destinations)} destinacija bez slike, izvor {SOURCE_NAME} "
        f"(pauza {settings.image_search_delay_seconds:g} s između poziva)."
    )

    used = _used_image_urls(conn)
    approximate: list[str] = []
    for index, destination in enumerate(destinations):
        if index > 0:
            time.sleep(settings.image_search_delay_seconds)
        _process(conn, settings, destination, used, reporter, approximate)

    if approximate:
        reporter.log(f"Približne slike (samo mjesto odgovara, zamijenite ručno): {', '.join(approximate)}.")
