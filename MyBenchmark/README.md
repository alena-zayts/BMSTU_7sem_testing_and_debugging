# wrk 
##  В общем
https://github.com/wg/wrk

wrk is a modern HTTP benchmarking tool capable of generating significant load when run on a single multi-core CPU.


* -с -- сколько HTTP соединений держатся открытыми
* -t -- сколько потоков
* -d -- длительность benchmark-а
* url -- куда
* --latency: print detailed latency statistics
* --timeout: record a timeout if a response is not received within this amount of time.
* -s, --script: LuaJIT script  
* -H задает header: {Host: хост-сервера, Accept: ..., Connection: keep-alive}

HTTP keep-alive -- использование одного TCP-соединения для отправки и получения многократных HTTP-запросов и ответов вместо открытия нового соединения для каждой пары запрос-ответ. Новый протокол HTTP/2 расширяет эту идею, позволяя одновременные многократные запросы/ответы в одном соединении.

Вызывается первый раз
```
wrk -H "Host: $server_host" -H "Accept: $accept" -H "Connection: keep-alive" --latency -d 5 -c 8 --timeout 8 -t 8 $url
```

Потом warm-up
```
wrk -H "Host: $server_host" -H "Accept: $accept" -H "Connection: keep-alive" --latency -d $duration -c $max_concurrency --timeout 8 -t $max_threads $url
```
* duration=15 -- константа на все 
* max_concurrency -- max(concurrencies to run)
* max_threads=$(cat /proc/cpuinfo | grep processor | wc -l) -- количество процессоров


Потом сами испытания

общие настройки для всех тестов:
``` 
'max_concurrency': max(self.config.concurrency_levels),
'name': name,
'duration': self.config.duration,
'server_host': self.config.server_host,
'url': url,
```

## JSON Serialization: JSON responses per second

Exercises the framework fundamentals including keep-alive support, request routing, request header parsing, object instantiation, JSON serialization, response header generation, and request count throughput.

### script
concurrency.sh

variables: levels = concurrency_levels, accept=json


### wrk
изменяется -с по уровням и -t=уровень > max_threads? max_threads : уровень


### calls:

aiohttp:
```python
aiohttp.web.json_response
json_response = partial(json_response, dumps=ujson.dumps)
return json_response({'message': 'Hello, World!'})
```

crax:
```python
crax.response_types.JSONResponse
return JSONResponse(None, {'message': 'Hello, world!'})
```

## Plaintext: Plaintext responses per second,

An exercise of the request-routing fundamentals only, designed to demonstrate the capacity of high-performance platforms in particular. Requests will be sent using HTTP pipelining. The response payload is still small, meaning good performance is still necessary in order to saturate the gigabit Ethernet of the test environment.

### script
pipeline.sh -> lua

variables: levels=pipeline_concurrency_levels, pipeline=16, accept=text/plain

The init() function receives any extra command line arguments for the
script which must be separated from wrk arguments with "--".

response() is called with the HTTP response status, headers, and body.

wrk.format returns an HTTP request string containing the passed parameters 
merged with values from the wrk table.

```
init = function(args)
  local r = {}
  local depth = tonumber(args[1]) or 1
  for i=1,depth do
    r[i] = wrk.format()
  end
  req = table.concat(r)
end

request = function()
  return req
end
```


### wrk
изменяется -с по уровням, -t=уровень > max_threads? max_threads : уровень
```
wrk -H "Host: $server_host" -H "Accept: $accept" -H "Connection: keep-alive" --latency -d $duration -c $c --timeout 8 -t "$(($c>$max_threads?$max_threads:$c))" $url -s pipeline.lua -- $pipeline
```



### calls
aiohttp:
```python
aiohttp.web.Response
return Response(body=b'Hello, World!', content_type='text/plain')
```

crax:
```python
crax.response_types.BaseResponse
return BaseResponse(None, b'Hello, world!')
```








## Multiple Query: Responses per second

Single Database Query: Exercises the framework's object-relational mapper (ORM), random number generator, database driver, and database connection pool.

Multiple Database Queries: A variation of Test #2 and also uses the World table. Multiple rows are fetched to more dramatically punish the database driver and connection pool. At the highest queries-per-request tested (20), this test demonstrates all frameworks' convergence toward zero requests-per-second as database activity increases.


### script
query.sh

variables: levels=query_levels, accept=application/json



### wrk

Изменяется -с по уровням и передается "$url$c"



### calls

aiohttp:
```python
async def multiple_database_queries(request):
    num_queries = get_num_queries(request)

    ids = [randint(1, 10000) for _ in range(num_queries)]

    result = []
    async with request.app['db_session']() as sess:
        for id_ in ids:
            num = await sess.scalar(READ_SELECT_ORM.filter_by(id=id_))
            result.append({'id': id_, 'randomNumber': num})
    return json_response(result)
```

crax:
```python
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
```




