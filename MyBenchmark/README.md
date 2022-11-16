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
duration=15 -- константа на все 

## JSON Serialization:

Exercises the framework fundamentals including keep-alive support, request routing, request header parsing, object instantiation, JSON serialization, response header generation, and request count throughput.

### script: concurrency.sh -> lua
variables: 

```buildoutcfg
'max_concurrency': max(self.config.concurrency_levels),
'name': name,
'duration': self.config.duration,
'levels': " ".join("{}".format(item) for item in self.config.concurrency_levels),
'server_host': self.config.server_host,
'url': url,
'accept': "application/json,
```

### calls:

aiohttp:
```buildoutcfg
aiohttp.web.json_response
json_response = partial(json_response, dumps=ujson.dumps)
return json_response({'message': 'Hello, World!'})
```

crax:
```buildoutcfg
crax.response_types.JSONResponse
return JSONResponse(None, {'message': 'Hello, world!'})
```






