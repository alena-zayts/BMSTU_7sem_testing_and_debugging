import os
import multiprocessing

import asyncpg
from aiohttp import web
from sqlalchemy.engine.url import URL
from sqlalchemy.ext.asyncio import AsyncSession, create_async_engine
from sqlalchemy.orm import sessionmaker

from .views import (
    json,
    single_database_query,
    multiple_database_queries,
    plaintext,
)


async def db_ctx(app: web.Application):
    # number of gunicorn workers = multiprocessing.cpu_count() as per gunicorn_conf.py
    # max_connections = 2000, give 10% leeway
    max_size = min(1800 / multiprocessing.cpu_count(), 160)
    max_size = max(int(max_size), 1)

    # DSN (Database Source Name) url suitable for sqlalchemy
    dsn = str(URL.create(
        database='hello_world',
        password=os.getenv('PGPASS', 'benchmarkdbpass'),
        host='bm-database',
        port='5432',
        username=os.getenv('PGUSER', 'benchmarkdbuser'),
        drivername='postgresql+{}'.format('asyncpg'),
    ))
    engine = create_async_engine(dsn, future=True, pool_size=max_size)
    app['db_session'] = sessionmaker(engine, class_=AsyncSession)

    yield


def setup_routes(app):
    app.router.add_get('/json', json)
    app.router.add_get('/db', single_database_query)
    app.router.add_get('/queries/{queries:.*}', multiple_database_queries)
    app.router.add_get('/plaintext', plaintext)


def create_app():
    app = web.Application()
    app.cleanup_ctx.append(db_ctx)
    setup_routes(app)
    return app

app = create_app()

