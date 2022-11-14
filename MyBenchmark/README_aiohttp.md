gunicorn -- это WSGI-сервер
WSGI-серверы появились потому, что веб-серверы в то время не умели взаимодействовать с приложениями, написанными на языке Python. WSGI (произносится как «whiz-gee»)

В простейшем случае WSGI состоит из двух основных сущностей:

Веб-сервер (Nginx, Apache и т. д.);
Веб-приложение, написанное на языке Python.

При размещении веб-приложения Python на продакшене вы не сможете обойтись без использования сервера WSGI и веб-сервера.

Gunicorn и Nginx - это самые надежные и популярные варианты таких приложений. Почему они используются в связке?

Nginx и Gunicorn работают вместе
Nginx принимает все запросы из Интернета. Он может обрабатывать их очень быстро и обычно настраивается так, чтобы пропускать только те запросы, которые действительно должны поступить в ваше веб-приложение. Остальные он блокирует.

Gunicorn переводит запросы, полученные от Nginx, в формат, который может обрабатывать ваше веб-приложение, и обеспечивает выполнение кода при необходимости.


# [aiohttp](http://aiohttp.readthedocs.io/) Benchmark Test

The information below is specific to aiohttp. For further guidance, 
review the [documentation](https://github.com/TechEmpower/FrameworkBenchmarks/wiki). 
Also note that there is additional information that's provided in 
the [Python README](../).

This is the Python aiohttp portion of a [benchmarking tests suite](../../) 
comparing a variety of frameworks.

All test implementations are located within ([./app](frameworks/Python/aiohttp/hello)).

## Description

aiohttp with [sqlalchemy](https://docs.sqlalchemy.org/en/14/orm/extensions/asyncio.html)  for database access.
 
[uvloop](https://github.com/MagicStack/uvloop) is used for a more performant event loop.

### Database

PostgreSQL.

ORM (not RAW) using [sqlalchemy](https://docs.sqlalchemy.org/en/14/orm/extensions/asyncio.html)

```
export CONNECTION=RAW
```
This will switch which database engine the app uses to execute queries with tests 2, 3, 4 & 5.

### Server

gunicorn+uvloop on CPython

## Test URLs

### Test 1: JSON Encoding 

    http://localhost:8080/json

### Test 2: Single Row Query

    http://localhost:8080/db

### Test 3: Multi Row Query 

    http://localhost:8080/queries/20

### Test 4: Plaintext

    http://localhost:8080/plaintext
