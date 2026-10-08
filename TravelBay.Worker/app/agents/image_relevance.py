"""How well a Wikimedia Commons file matches a destination, judged from the file's title and from
its object name, description and categories. Text is compared without diacritics and by word
prefixes, so "Baščaršije" (Bosnian case ending) still matches a file about "Bascarsija".

A destination name is split into identity words (Una, Kravica, Baščaršija — what makes it *this*
place) and type words (park, plaža, vinski podrumi — what kind of place it is). Type words count as
concepts together with the descriptive AI keywords (wine cellar, seafood, ...). Country and region
names (Bosna, Hercegovina, ...) never count: they appear on half of Commons.

- GOOD:        the identity (or the city, when the name has no other identity) together with what
               kind of place it is; for names without type words, most identity words are enough;
- APPROXIMATE: only the place matches (e.g. a view of the town for a restaurant);
- MISS:        neither the identity nor the city appears — an obvious miss, never used.
A match in the file title outweighs one found only in its description or categories."""

import re
import unicodedata
from dataclasses import dataclass
from enum import Enum

STEM_LENGTH = 5
MIN_WORD_LENGTH = 3
MIN_CONCEPT_LENGTH = 4
MAX_COUNTED_CONCEPTS = 3
GOOD_IDENTITY_RATIO = 0.5

IDENTITY_WEIGHT = 4.0
TITLE_IDENTITY_WEIGHT = 3.0
CITY_WEIGHT = 2.0
CONCEPT_WEIGHT = 1.0

# Words that say nothing about a particular place or thing.
STOPWORDS = {
    "and", "the", "with", "from", "near", "over", "for", "bosnia", "herzegovina", "croatia",
    "montenegro", "bosnian", "herzegovinian", "national", "popular", "traditional", "town", "city",
    "view", "old", "new", "game", "shaped", "like", "home", "last",
}

# Country and region names: they match half of Commons, so they never count as a concept.
REGION_STEMS = {"bosna", "bosni", "bosan", "herce", "herze", "crna", "gora", "hrvat", "monte", "adria", "jadra"}

# Stems of Bosnian words that name the kind of destination, not the destination itself, with the
# stems of their English equivalents (most Commons titles and categories are in English).
TYPE_STEMS = {
    "nacio": [], "park": [], "stari": [], "grad": [], "speci": [], "tura": [],
    "plaza": ["beach"],
    "vrelo": ["sprin", "sourc"],
    "vodop": ["water", "fall"],
    "jezer": ["lake"],
    "kulin": ["culin", "cuisi", "food"],
    "vinsk": ["wine", "winer", "vinar"],
    "podru": ["cella"],
    "morsk": ["seafo", "fish"],
    "cevab": ["cevap", "kebab"],
    "tvrda": ["fortr", "castl"],
}


class Match(Enum):
    GOOD = "dobra"
    APPROXIMATE = "približna"
    MISS = "promašaj"


@dataclass(frozen=True)
class Relevance:
    match: Match
    score: float
    concept_hits: int
    # The destination (or, without identity words, its city) is named in the file title itself.
    in_title: bool


def normalize(text: str) -> str:
    text = text.replace("đ", "d").replace("Đ", "D")
    decomposed = unicodedata.normalize("NFKD", text)
    ascii_text = "".join(ch for ch in decomposed if not unicodedata.combining(ch))
    return re.sub(r"[^a-z0-9]+", " ", ascii_text.lower()).strip()


def stems(text: str, min_length: int = MIN_WORD_LENGTH) -> list[str]:
    result: list[str] = []
    for word in normalize(text).split():
        if len(word) >= min_length and word not in STOPWORDS:
            stem = word[:STEM_LENGTH]
            if stem not in result:
                result.append(stem)
    return result


def is_type_word(word: str) -> bool:
    return normalize(word)[:STEM_LENGTH] in TYPE_STEMS


def _contains(text: str, stem: str) -> bool:
    return re.search(rf"\b{re.escape(stem)}", text) is not None


def _ratio(text: str, words: list[str]) -> float:
    return sum(_contains(text, s) for s in words) / len(words) if words else 0.0


@dataclass(frozen=True)
class DestinationTerms:
    identity: list[str]
    city: list[str]
    concepts: list[str]
    # What kind of place the name says it is (park, plaža, morski specijaliteti …) with its English
    # equivalents; when the name has one, a GOOD image must show exactly that, not just any keyword.
    type_concepts: list[str]

    @staticmethod
    def of(name: str, city: str, country: str, keywords: list[str]) -> "DestinationTerms":
        city_stems = stems(city)
        name_stems = stems(name)
        # "Hercegovačka kulinarska tura": a region in the name identifies nothing.
        identity = [s for s in name_stems if s not in TYPE_STEMS and s not in city_stems and s not in REGION_STEMS]
        type_concepts: list[str] = []
        for stem in (s for s in name_stems if s in TYPE_STEMS):
            type_concepts += [c for c in [stem, *TYPE_STEMS[stem]] if c not in type_concepts]
        concepts = list(type_concepts)
        known = set(name_stems) | set(city_stems) | set(stems(country)) | REGION_STEMS
        # The first keyword is the name + place; the others describe the destination.
        for keyword in keywords[1:]:
            for stem in stems(keyword, MIN_CONCEPT_LENGTH):
                if stem not in known and stem not in concepts:
                    concepts.append(stem)
        return DestinationTerms(identity, city_stems, concepts, type_concepts)


def judge(terms: DestinationTerms, title: str, details: str) -> Relevance:
    """[title] is the file name, [details] its object name, description and categories."""
    title_text = normalize(title)
    text = f"{title_text} {normalize(details)}"
    identity_ratio = _ratio(text, terms.identity)
    title_identity_ratio = _ratio(title_text, terms.identity)
    city_hit = any(_contains(text, s) for s in terms.city)
    concept_hits = min(sum(_contains(text, s) for s in terms.concepts), MAX_COUNTED_CONCEPTS)
    type_hit = any(_contains(text, s) for s in terms.type_concepts)

    score = (IDENTITY_WEIGHT * identity_ratio + TITLE_IDENTITY_WEIGHT * title_identity_ratio
             + CITY_WEIGHT * city_hit + CONCEPT_WEIGHT * concept_hits)

    if terms.identity:
        in_title = title_identity_ratio > 0
        identified = identity_ratio > 0
    else:
        # e.g. "Vinski podrumi Trebinja": only the city identifies it.
        in_title = any(_contains(title_text, s) for s in terms.city)
        identified = city_hit

    if terms.type_concepts:
        good = identified and type_hit
    elif terms.identity:
        good = identity_ratio >= GOOD_IDENTITY_RATIO or (identified and concept_hits > 0)
    else:
        good = identified and concept_hits > 0

    if good:
        return Relevance(Match.GOOD, score, concept_hits, in_title)
    if city_hit or identity_ratio > 0:
        return Relevance(Match.APPROXIMATE, score, concept_hits, in_title)
    return Relevance(Match.MISS, score, concept_hits, in_title)
