"""Worker configuration. Every value comes from the environment (.env via docker-compose env_file);
nothing secret has a default here."""

import os
from dataclasses import dataclass

from dotenv import load_dotenv

# Running outside Docker (python main.py from TravelBay.Worker/) picks up the repo-root .env.
# Inside the container there is no .env file: docker-compose passes the variables directly.
load_dotenv(os.path.join(os.path.dirname(__file__), "..", "..", ".env"))


class ConfigError(RuntimeError):
    """A required setting is missing; the message names it."""


def _required(name: str) -> str:
    value = os.getenv(name, "").strip()
    if not value:
        raise ConfigError(f"{name} is not set in .env")
    return value


def _optional(name: str) -> str:
    return os.getenv(name, "").strip()


@dataclass(frozen=True)
class Settings:
    db_host: str
    db_port: int
    db_name: str
    db_user: str
    db_password: str

    rabbitmq_host: str
    rabbitmq_port: int
    rabbitmq_user: str
    rabbitmq_password: str

    # Checked only when the keywords agent runs, so a missing key fails that run with a clear
    # log line instead of stopping the whole worker. The image agent (Wikimedia Commons) needs no key.
    groq_api_key: str
    groq_model: str
    image_search_delay_seconds: float


# The API connects as sa as well (Program.cs); the password is DB_SA_PASSWORD from .env.
DB_USER = "sa"


def load_settings() -> Settings:
    return Settings(
        db_host=_required("DB_HOST"),
        db_port=int(_required("DB_PORT")),
        db_name=_required("DB_NAME"),
        db_user=DB_USER,
        db_password=_required("DB_SA_PASSWORD"),
        rabbitmq_host=_required("RABBITMQ_HOST"),
        rabbitmq_port=int(_required("RABBITMQ_PORT")),
        rabbitmq_user=_required("RABBITMQ_DEFAULT_USER"),
        rabbitmq_password=_required("RABBITMQ_DEFAULT_PASS"),
        groq_api_key=_optional("GROQ_API_KEY"),
        groq_model=_required("GROQ_MODEL"),
        image_search_delay_seconds=float(_required("IMAGE_SEARCH_DELAY_SECONDS")),
    )
