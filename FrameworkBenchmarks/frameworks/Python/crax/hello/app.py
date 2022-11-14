import os
from operator import itemgetter
from random import randint
import asyncpg
from crax import Crax
from crax.response_types import BaseResponse, JSONResponse
from crax.urls import Route, Url
from crax.views import JSONView, TemplateView

READ_ROW_SQL = 'SELECT "id", "randomnumber" FROM "world" WHERE id = $1'
WRITE_ROW_SQL = 'UPDATE "world" SET "randomnumber"=$1 WHERE id=$2'


async def setup_database():
    global connection_pool
    connection_pool = await asyncpg.create_pool(
        user=os.getenv('PGUSER', 'benchmarkdbuser'),
        password=os.getenv('PGPASS', 'benchmarkdbpass'),
        database='hello_world',
        host='bm-database',
        port=5432
    )


def get_num_queries(request):
    try:
        query_count = int(request.query["queries"][0])
    except (KeyError, IndexError, ValueError):
        return 1
    if query_count < 1:
        return 1
    if query_count > 500:
        return 500
    return query_count


class TestSingleQuery(JSONView):
    async def get(self):
        row_id = randint(1, 10000)
        async with connection_pool.acquire() as connection:
            if self.request.path == '/db':
                res = await connection.fetchval(READ_ROW_SQL, row_id)
                self.context = {'id': row_id, 'randomNumber': res}


class TestMultiQueries(JSONView):
    async def get(self):
        row_ids = [randint(1, 10000) for _ in range(get_num_queries(self.request))]
        worlds = []
        async with connection_pool.acquire() as connection:
            statement = await connection.prepare(READ_ROW_SQL)
            for row_id in row_ids:
                number = await statement.fetchval(row_id)
                worlds.append({'id': row_id, 'randomNumber': number})
            self.context = worlds


APPLICATIONS = ["hello"]
URL_PATTERNS = [
    Route(Url('/json'), JSONResponse(None, {'message': 'Hello, world!'})),
    Route(Url('/plaintext'), BaseResponse(None, b'Hello, world!')),
    Route(Url('/db'), TestSingleQuery),
    Route(Url('/queries'), TestMultiQueries),
]
app = Crax('hello.hello', debug=True, on_startup=setup_database)
