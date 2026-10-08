"""SQL Server access for the agents (TravelBay database). pymssql needs no ODBC driver in the image."""

import pymssql

from .config import Settings


def connect(settings: Settings) -> pymssql.Connection:
    return pymssql.connect(
        server=settings.db_host,
        port=settings.db_port,
        user=settings.db_user,
        password=settings.db_password,
        database=settings.db_name,
        login_timeout=15,
        autocommit=False,
    )
