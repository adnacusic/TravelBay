"""Emails sent for the API over SMTP (settings from .env). The only email today is the password
reset code; the code itself is never logged."""

import json
import logging
import smtplib
import ssl
from email.message import EmailMessage

from .config import Settings
from .messages import SEND_PASSWORD_RESET_CODE

logger = logging.getLogger(__name__)

SMTP_TIMEOUT_SECONDS = 30
SUBJECT = "TravelBay — kod za promjenu lozinke"


def process_email_job(settings: Settings, body: bytes) -> None:
    try:
        message = json.loads(body)
        command = message["command"]
    except (ValueError, KeyError, TypeError):
        logger.error("Ignoring malformed email message.")
        return

    if command != SEND_PASSWORD_RESET_CODE:
        logger.error("Unknown email command %r, message dropped.", command)
        return

    if not settings.smtp_configured:
        logger.error("Password reset email not sent: SMTP is not configured (SMTP_HOST / SMTP_FROM in .env).")
        return

    try:
        email = message["email"]
        first_name = message.get("firstName") or ""
        code = message["code"]
        valid_minutes = int(message["validMinutes"])
    except (KeyError, TypeError, ValueError):
        logger.error("Ignoring incomplete password reset message.")
        return

    _send(settings, _password_reset_email(settings.smtp_from, email, first_name, code, valid_minutes))
    logger.info("Password reset email sent.")


def _password_reset_email(sender: str, to: str, first_name: str, code: str, valid_minutes: int) -> EmailMessage:
    greeting = f"Zdravo {first_name}," if first_name else "Zdravo,"
    text = (
        f"{greeting}\n\n"
        f"vaš kod za promjenu lozinke na TravelBayu je: {code}\n\n"
        f"Kod vrijedi {valid_minutes} minuta i može se iskoristiti jednom. Unesite ga u aplikaciji "
        "na ekranu \"Nova lozinka\".\n\n"
        "Ako niste vi zatražili promjenu lozinke, zanemarite ovu poruku — vaša lozinka ostaje ista.\n\n"
        "TravelBay"
    )
    html = (
        f"<p>{greeting}</p>"
        "<p>vaš kod za promjenu lozinke na TravelBayu je:</p>"
        f"<p style=\"font-size:28px;font-weight:bold;letter-spacing:6px\">{code}</p>"
        f"<p>Kod vrijedi {valid_minutes} minuta i može se iskoristiti jednom.</p>"
        "<p>Ako niste vi zatražili promjenu lozinke, zanemarite ovu poruku — vaša lozinka ostaje ista.</p>"
        "<p>TravelBay</p>"
    )
    email = EmailMessage()
    email["Subject"] = SUBJECT
    email["From"] = sender
    email["To"] = to
    email.set_content(text)
    email.add_alternative(html, subtype="html")
    return email


def _send(settings: Settings, email: EmailMessage) -> None:
    """SMTP_USE_SSL=true: implicit TLS (usually port 465); otherwise STARTTLS when the server offers it
    (e.g. Mailtrap sandbox on 2525/587)."""
    context = ssl.create_default_context()
    if settings.smtp_use_ssl:
        server: smtplib.SMTP = smtplib.SMTP_SSL(
            settings.smtp_host, settings.smtp_port, timeout=SMTP_TIMEOUT_SECONDS, context=context)
    else:
        server = smtplib.SMTP(settings.smtp_host, settings.smtp_port, timeout=SMTP_TIMEOUT_SECONDS)
    with server:
        server.ehlo()
        if not settings.smtp_use_ssl and server.has_extn("starttls"):
            server.starttls(context=context)
            server.ehlo()
        if settings.smtp_user:
            server.login(settings.smtp_user, settings.smtp_password)
        server.send_message(email)
