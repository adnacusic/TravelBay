"""AIAgentKeywords: Groq (Llama 3.1) writes 3-5 English keywords for every destination whose
Keywords is still empty. Stored comma-separated in Destinations.Keywords; the clients split on commas,
and the first keyword (name + place) is the image search query of AIAgentSlike."""

import json
import re
import time

import groq
import pymssql

from ..config import Settings
from ..runs import RunReporter
from . import FatalAgentError

MIN_KEYWORDS = 3
MAX_KEYWORDS = 5
MAX_KEYWORD_LENGTH = 60
TEMPERATURE = 0.2

# Groq's free tier allows about 30 requests per minute; a short pause keeps a long run under it.
PAUSE_SECONDS = 1.0

PROMPT = """You are a travel guide assistant for Bosnia and Herzegovina and the region.

Return ONLY valid JSON. No text, no explanation, no markdown.

Format EXACTLY like this:
["keyword 1", "keyword 2", "keyword 3", "keyword 4", "keyword 5"]

Rules:
- Generate 3 to 5 concise keywords in English
- The FIRST keyword is the destination name with its city and country (it is used as an image search query)
- The others describe the location and what the destination is known for
- No commas inside a keyword

Destination: {name}
Category: {category}
Location: {city}, {country}
Description: {description}
"""


def _pending_destinations(conn: pymssql.Connection) -> list[dict]:
    with conn.cursor(as_dict=True) as cursor:
        cursor.execute(
            """SELECT d.Id, d.Name, d.Description, c.Name AS City, co.Name AS Country, cat.Name AS Category
               FROM Destinations d
               JOIN Cities c ON c.Id = d.CityId
               JOIN Countries co ON co.Id = c.CountryId
               JOIN Categories cat ON cat.Id = d.CategoryId
               WHERE d.IsDeleted = 0 AND (d.Keywords IS NULL OR LTRIM(RTRIM(d.Keywords)) = N'')
               ORDER BY d.Id"""
        )
        return cursor.fetchall()


def parse_keywords(text: str) -> list[str]:
    """JSON array if the model kept to the format; otherwise the text inside [...] split on commas
    (same fallback as the original agent). Duplicates and over-long entries are dropped."""
    match = re.search(r"\[.*\]", text, re.DOTALL)
    candidate = match.group(0) if match else text
    try:
        parsed = json.loads(candidate)
        items = parsed if isinstance(parsed, list) else []
    except json.JSONDecodeError:
        items = candidate.replace("[", "").replace("]", "").replace('"', "").split(",")

    keywords: list[str] = []
    for item in items:
        keyword = " ".join(str(item).replace(",", " ").split())
        if keyword and len(keyword) <= MAX_KEYWORD_LENGTH and keyword.lower() not in (k.lower() for k in keywords):
            keywords.append(keyword)
    return keywords[:MAX_KEYWORDS]


def _ask_groq(client: groq.Groq, model: str, destination: dict) -> str:
    prompt = PROMPT.format(
        name=destination["Name"],
        category=destination["Category"],
        city=destination["City"],
        country=destination["Country"],
        description=destination["Description"],
    )
    response = client.chat.completions.create(
        model=model,
        messages=[{"role": "user", "content": prompt}],
        temperature=TEMPERATURE,
    )
    return response.choices[0].message.content or ""


def _save(conn: pymssql.Connection, destination_id: int, keywords: list[str]) -> None:
    # Only fills a still-empty value, so an admin's manual edit made meanwhile is never overwritten.
    with conn.cursor() as cursor:
        cursor.execute(
            """UPDATE Destinations SET Keywords = %s, UpdatedAt = SYSUTCDATETIME()
               WHERE Id = %d AND (Keywords IS NULL OR LTRIM(RTRIM(Keywords)) = N'')""",
            (", ".join(keywords), destination_id),
        )
    conn.commit()


def run(conn: pymssql.Connection, settings: Settings, reporter: RunReporter) -> None:
    if not settings.groq_api_key:
        raise FatalAgentError("GROQ_API_KEY nije postavljen u .env — AIAgentKeywords ne može raditi.")

    destinations = _pending_destinations(conn)
    reporter.start(len(destinations))
    reporter.log(f"AIAgentKeywords: {len(destinations)} destinacija bez ključnih riječi (model {settings.groq_model}).")

    client = groq.Groq(api_key=settings.groq_api_key)
    for index, destination in enumerate(destinations):
        name = destination["Name"]
        try:
            content = _ask_groq(client, settings.groq_model, destination)
        except groq.AuthenticationError as ex:
            raise FatalAgentError("Groq je odbio GROQ_API_KEY (neispravan ključ).") from ex
        except groq.APIError as ex:
            reporter.log(f"Greška za {name}: Groq poziv nije uspio ({ex.__class__.__name__}).")
            reporter.item_done(success=False)
            continue

        keywords = parse_keywords(content)
        if len(keywords) < MIN_KEYWORDS:
            reporter.log(f"Greška za {name}: model nije vratio bar {MIN_KEYWORDS} ključne riječi (odgovor: {content[:120]!r}).")
            reporter.item_done(success=False)
        else:
            _save(conn, destination["Id"], keywords)
            reporter.log(f"{name}: {', '.join(keywords)}")
            reporter.item_done(success=True)

        if index < len(destinations) - 1:
            time.sleep(PAUSE_SECONDS)
