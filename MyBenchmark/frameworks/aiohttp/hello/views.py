from functools import partial
from random import randint

import ujson
from aiohttp.web import Response, json_response
from sqlalchemy import select

from sqlalchemy import Column, Integer
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()
class World(Base):
    __tablename__ = 'world'
    id = Column(Integer, primary_key=True)
    randomnumber = Column(Integer)


READ_SELECT_ORM = select(World.randomnumber)
json_response = partial(json_response, dumps=ujson.dumps)


def get_num_queries(request):
    try:
        num_queries = int(request.match_info.get('queries', 1))
    except ValueError:
        return 1
    if num_queries < 1:
        return 1
    if num_queries > 500:
        return 500
    return num_queries


async def json(request):
    return json_response({'message': 'Hello, World!'})


async def single_database_query(request):
    id_ = randint(1, 10000)
    async with request.app['db_session']() as sess:
        num = await sess.scalar(select(World.randomnumber).filter_by(id=id_))
    return json_response({'id': id_, 'randomNumber': num})


async def multiple_database_queries(request):
    num_queries = get_num_queries(request)

    ids = [randint(1, 10000) for _ in range(num_queries)]

    result = []
    async with request.app['db_session']() as sess:
        for id_ in ids:
            num = await sess.scalar(READ_SELECT_ORM.filter_by(id=id_))
            result.append({'id': id_, 'randomNumber': num})
    return json_response(result)


async def plaintext(request):
    return Response(body=b'Hello, World!', content_type='text/plain')
