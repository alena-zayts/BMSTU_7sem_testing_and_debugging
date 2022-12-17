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

## JSON Serialization: число JSON ответов в секунду

Проверяет основы фреймворка, включая поддержку keep-alive, маршрутизацию запросов, синтаксический анализ заголовка запроса, создание экземпляра объекта, сериализацию JSON, генерацию заголовка ответа и пропускную способность.


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

## Plaintext: число Plaintext ответов в секунду
Проверяет только по основы маршрутизации запросов, предназначено для демонстрации возможностей высокопроизводительных платформ. Запросы будут отправляться с использованием конвейерной обработки HTTP. 
HTTP pipelining is enabled and higher client-side concurrency levels are used

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








## Multiple Query: число ответов в секунду

Проверяется объектно-реляционное сопоставление фреймворка (ORM), генератор случайных чисел, драйвер базы данных и пул подключений к базе данных. Извлекается несколько строк, чтобы более эффективно нагрузить драйвер базы данных и пул подключений. 


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


# Gunicorn 

WSGI (англ. Web Server Gateway Interface) — стандарт взаимодействия между Python-программой, выполняющейся на стороне сервера, и самим веб-сервером

ASGI (Asynchronous Server Gateway Interface)

Веб-сервер, в нашем случае Nginx, принимает и обрабатывает HTTP-запрос браузера, затем передаёт его в Application-сервер — Gunicorn.
Gunicorn получает данные от Nginx, разбирает их и исходя из своей конфигурации по протоколу WSGI передаёт их в Django.
Django обрабатывает полученные данные и возвращает результат работы обратно в Gunicorn, а он в свою очередь отдаёт результат в Nginx, который возвращает клиенту готовую HTML-страницу.

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

# more

aiohttp with [sqlalchemy] for database access.

# Results

## plaintext

Sorted by requestsPerSecond
```
  framework  plaintextConcurrencyLevels  totalRequests  seconds  requestsPerSecond  latencyAvg  latencyMax
0   aiohttp                         256       626258.2     15.0       41750.546667      61.274      57.934
1   aiohttp                        1024       584364.6     15.2       38462.738333     229.756     179.186
2      crax                         256       376826.6     15.2       24824.633333      98.626     120.270
3      crax                        1024       349369.6     15.0       23291.306667     364.840       0.320
```

Sorted by latencyAvg
```

  framework  plaintextConcurrencyLevels  totalRequests  seconds  requestsPerSecond  latencyAvg  latencyMax
0   aiohttp                         256       626258.2     15.0       41750.546667      61.274      57.934
2      crax                         256       376826.6     15.2       24824.633333      98.626     120.270
1   aiohttp                        1024       584364.6     15.2       38462.738333     229.756     179.186
3      crax                        1024       349369.6     15.0       23291.306667     364.840       0.320

```

## json

Sorted by requestsPerSecond
```
  framework  jsonConcurrencyLevels  totalRequests  seconds  requestsPerSecond  latencyAvg  latencyMax
2   aiohttp                    128       467888.2     15.0       31192.546667       4.664      10.234
1   aiohttp                     32       395850.4     15.0       26390.026667       1.888       8.214
5      crax                    128       362972.4     15.4       23592.465833       5.716      22.526
0   aiohttp                     16       347400.4     15.2       22882.076667       1.099       6.076
4      crax                     32       313170.4     15.0       20878.026667       1.996       8.590
3      crax                     16       295702.2     15.2       19467.260833       1.160       8.082
```

Sorted by latencyAvg
```
  framework  jsonConcurrencyLevels  totalRequests  seconds  requestsPerSecond  latencyAvg  latencyMax
0   aiohttp                     16       347400.4     15.2       22882.076667       1.099       6.076
3      crax                     16       295702.2     15.2       19467.260833       1.160       8.082
1   aiohttp                     32       395850.4     15.0       26390.026667       1.888       8.214
4      crax                     32       313170.4     15.0       20878.026667       1.996       8.590
2   aiohttp                    128       467888.2     15.0       31192.546667       4.664      10.234
5      crax                    128       362972.4     15.4       23592.465833       5.716      22.526
```


## query
Sorted by requestsPerSecond
```

  framework  queryLevels  totalRequests  seconds  requestsPerSecond  latencyAvg  latencyMax
3      crax            1        84023.8     15.0        5601.586667      25.700      72.772
0   aiohttp            1        58368.0     15.0        3891.200000      34.278      82.180
4      crax           10        35491.4     15.0        2366.093333      56.656     114.594
5      crax           20        24197.4     15.0        1613.160000      80.592     101.530
1   aiohttp           10        11385.4     15.2         750.574167     170.912     186.710
2   aiohttp           20         6347.2     15.4         412.698333     304.884     175.472

```
Sorted by latencyAvg
```
  framework  queryLevels  totalRequests  seconds  requestsPerSecond  latencyAvg  latencyMax
3      crax            1        84023.8     15.0        5601.586667      25.700      72.772
0   aiohttp            1        58368.0     15.0        3891.200000      34.278      82.180
4      crax           10        35491.4     15.0        2366.093333      56.656     114.594
5      crax           20        24197.4     15.0        1613.160000      80.592     101.530
1   aiohttp           10        11385.4     15.2         750.574167     170.912     186.710
2   aiohttp           20         6347.2     15.4         412.698333     304.884     175.472
```


По plaintext и json выигрывает aiohttp, по query -- crax


orm - object relational mapper 

В aiohttp -- full, в crax -- raw

The ORM is a common conversion point among the majority of developers who work with Django. Most Django developers are going to have a comfort level with the Django orm. Most tutorials, plugins, and examples leverage the orm to interface with the database. I've even seen some junior devs who don't even know SQL queries and only know how to use the ORM.

RAW SQL will be more efficient in a lot of cases, however you then bypass the abstraction benefits the ORM provides.

If you truly care about optimizing then RAW SQL will be inevitable, but if you care more about (depending on the team) consistency and leveraging the abstraction then stick with the ORM.