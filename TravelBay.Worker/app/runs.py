"""Progress of one AI agent run, written into the AiAgentRuns row the API created.
The desktop "AI Agenti" screen polls that row, so every step is committed right away."""

import logging
from datetime import datetime, timezone

import pymssql

from .messages import RunStatus

logger = logging.getLogger(__name__)


class RunReporter:
    def __init__(self, conn: pymssql.Connection, run_id: int):
        self._conn = conn
        self.run_id = run_id
        self.processed = 0
        self.succeeded = 0
        self.failed = 0

    def status(self) -> RunStatus | None:
        """None when the run does not exist (e.g. a message for a deleted database)."""
        with self._conn.cursor() as cursor:
            cursor.execute("SELECT Status FROM AiAgentRuns WHERE Id = %d", (self.run_id,))
            row = cursor.fetchone()
        return None if row is None else RunStatus(row[0])

    def start(self, total: int) -> None:
        with self._conn.cursor() as cursor:
            cursor.execute(
                """UPDATE AiAgentRuns
                   SET Status = %d, StartedAt = SYSUTCDATETIME(), TotalCount = %d,
                       ProcessedCount = 0, SucceededCount = 0, FailedCount = 0
                   WHERE Id = %d""",
                (int(RunStatus.RUNNING), total, self.run_id),
            )
        self._conn.commit()

    def log(self, line: str) -> None:
        stamped = f"[{datetime.now(timezone.utc):%H:%M:%S}] {line}"
        logger.info("run %s: %s", self.run_id, line)
        with self._conn.cursor() as cursor:
            cursor.execute(
                "UPDATE AiAgentRuns SET Log = Log + NCHAR(10) + %s WHERE Id = %d",
                (stamped, self.run_id),
            )
        self._conn.commit()

    def item_done(self, success: bool) -> None:
        self.processed += 1
        if success:
            self.succeeded += 1
        else:
            self.failed += 1
        with self._conn.cursor() as cursor:
            cursor.execute(
                """UPDATE AiAgentRuns
                   SET ProcessedCount = %d, SucceededCount = %d, FailedCount = %d
                   WHERE Id = %d""",
                (self.processed, self.succeeded, self.failed, self.run_id),
            )
        self._conn.commit()

    def finish(self, status: RunStatus, line: str) -> None:
        self.log(line)
        with self._conn.cursor() as cursor:
            cursor.execute(
                "UPDATE AiAgentRuns SET Status = %d, FinishedAt = SYSUTCDATETIME() WHERE Id = %d",
                (int(status), self.run_id),
            )
        self._conn.commit()
