"""RabbitMQ consumer: one job at a time from the durable queue, acknowledged only after the run is
written to the database. A job runs in its own thread so pika keeps answering broker heartbeats
during long runs (the image agent pauses between Google calls)."""

import functools
import json
import logging
import threading
import time

import pika
import pika.exceptions

from . import db
from .agents import FatalAgentError, images_agent, keywords_agent
from .config import Settings
from .messages import COMMAND_AGENT, QUEUE_NAME, AgentType, RunStatus
from .runs import RunReporter

logger = logging.getLogger(__name__)

RECONNECT_DELAY_SECONDS = 5
HEARTBEAT_SECONDS = 60
# While the broker is still booting a connect can stall; these make it fail and retry instead.
SOCKET_TIMEOUT_SECONDS = 10
STACK_TIMEOUT_SECONDS = 20
BLOCKED_CONNECTION_TIMEOUT_SECONDS = 60

AGENTS = {
    AgentType.KEYWORDS: keywords_agent.run,
    AgentType.IMAGES: images_agent.run,
}


def process_job(settings: Settings, body: bytes) -> None:
    """Never raises: every outcome ends up in the run row (or the worker log for a bad message)."""
    try:
        message = json.loads(body)
        agent = COMMAND_AGENT[message["command"]]
        run_id = int(message["runId"])
    except (ValueError, KeyError, TypeError):
        logger.error("Ignoring malformed message: %r", body[:200])
        return

    conn = db.connect(settings)
    try:
        reporter = RunReporter(conn, run_id)
        status = reporter.status()
        if status is None:
            logger.warning("Run %s does not exist, message dropped.", run_id)
            return
        if status in (RunStatus.COMPLETED, RunStatus.FAILED):
            logger.info("Run %s is already %s, redelivered message skipped.", run_id, status.name)
            return
        if status == RunStatus.RUNNING:
            reporter.log("Worker je ponovo pokrenut tokom obrade — nastavljam s preostalim destinacijama.")
        else:
            reporter.log("Worker je preuzeo poruku iz RabbitMQ reda.")

        try:
            AGENTS[agent](conn, settings, reporter)
        except FatalAgentError as ex:
            reporter.finish(RunStatus.FAILED, f"Prekinuto: {ex}")
            return
        except Exception:
            logger.exception("Run %s failed unexpectedly.", run_id)
            conn.rollback()
            reporter.finish(RunStatus.FAILED, "Prekinuto zbog neočekivane greške u workeru (detalji u logu kontejnera).")
            return

        reporter.finish(
            RunStatus.COMPLETED,
            f"Završeno: obrađeno {reporter.processed}, uspješno {reporter.succeeded}, neuspješno {reporter.failed}.",
        )
    finally:
        conn.close()


def _ack(channel, delivery_tag: int) -> None:
    if channel.is_open:
        channel.basic_ack(delivery_tag)


def _work(connection, channel, delivery_tag: int, settings: Settings, body: bytes) -> None:
    try:
        process_job(settings, body)
    except Exception:
        # Database unreachable etc.: the message is still acknowledged, otherwise the same job
        # would be redelivered forever. The run stays Queued/Running and the admin can see it.
        logger.exception("Job could not be processed.")
    connection.add_callback_threadsafe(functools.partial(_ack, channel, delivery_tag))


def _consume_once(settings: Settings) -> None:
    parameters = pika.ConnectionParameters(
        host=settings.rabbitmq_host,
        port=settings.rabbitmq_port,
        credentials=pika.PlainCredentials(settings.rabbitmq_user, settings.rabbitmq_password),
        heartbeat=HEARTBEAT_SECONDS,
        socket_timeout=SOCKET_TIMEOUT_SECONDS,
        stack_timeout=STACK_TIMEOUT_SECONDS,
        blocked_connection_timeout=BLOCKED_CONNECTION_TIMEOUT_SECONDS,
        client_properties={"connection_name": "travelbay-worker"},
    )
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()
    channel.queue_declare(queue=QUEUE_NAME, durable=True)
    channel.basic_qos(prefetch_count=1)

    threads: list[threading.Thread] = []

    def on_message(ch, method, _properties, body):
        thread = threading.Thread(
            target=_work, args=(connection, ch, method.delivery_tag, settings, body), daemon=True
        )
        thread.start()
        threads.append(thread)

    channel.basic_consume(queue=QUEUE_NAME, on_message_callback=on_message)
    logger.info("Listening on RabbitMQ queue '%s' at %s:%s.", QUEUE_NAME, settings.rabbitmq_host, settings.rabbitmq_port)
    try:
        channel.start_consuming()
    finally:
        for thread in threads:
            thread.join()
        if connection.is_open:
            connection.close()


def consume_forever(settings: Settings) -> None:
    """RabbitMQ may start after the worker or restart later: keep reconnecting. OSError covers the
    broker host name not resolving while its container is down (socket.gaierror)."""
    while True:
        try:
            _consume_once(settings)
        except (pika.exceptions.AMQPConnectionError, OSError) as ex:
            logger.warning("RabbitMQ not reachable (%s), retrying in %s s.", ex.__class__.__name__, RECONNECT_DELAY_SECONDS)
            time.sleep(RECONNECT_DELAY_SECONDS)
