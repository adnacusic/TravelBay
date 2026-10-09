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

    # Emails ("Zaboravljena lozinka"). Optional so the worker still runs the agents without SMTP;
    # the API refuses the request with a clear message when SMTP_HOST/SMTP_FROM are missing.
    smtp_host: str
    smtp_port: int
    smtp_user: str
    smtp_password: str
    smtp_from: str
    smtp_use_ssl: bool

    @property
    def smtp_configured(self) -> bool:
        return bool(self.smtp_host and self.smtp_from)


# The API connects as sa as well (Program.cs); the password is DB_SA_PASSWORD from .env.
DB_USER = "sa"

# Submission port with STARTTLS; implicit TLS (SMTP_USE_SSL=true) normally uses 465.
DEFAULT_SMTP_PORT = 587
TRUE_VALUES = {"1", "true", "yes", "da"}


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
        smtp_host=_optional("SMTP_HOST"),
        smtp_port=int(_optional("SMTP_PORT") or DEFAULT_SMTP_PORT),
        smtp_user=_optional("SMTP_USER"),
        smtp_password=_optional("SMTP_PASS"),
        smtp_from=_optional("SMTP_FROM"),
        smtp_use_ssl=_optional("SMTP_USE_SSL").lower() in TRUE_VALUES,
    )
