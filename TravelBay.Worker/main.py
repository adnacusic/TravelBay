"""TravelBay AI worker: a separate service that consumes AI agent jobs from RabbitMQ."""

import logging
import sys

from app.config import ConfigError, load_settings
from app.consumer import consume_forever


def main() -> int:
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s %(levelname)s %(name)s: %(message)s",
        stream=sys.stdout,
    )
    # pika logs every failed connect with a traceback; the consumer logs one line per retry itself.
    logging.getLogger("pika").setLevel(logging.CRITICAL)
    logging.getLogger("httpx").setLevel(logging.WARNING)

    try:
        settings = load_settings()
    except ConfigError as ex:
        logging.critical("Configuration error: %s", ex)
        return 1

    consume_forever(settings)
    return 0


if __name__ == "__main__":
    sys.exit(main())
