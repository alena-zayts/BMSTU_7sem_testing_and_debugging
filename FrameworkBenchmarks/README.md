## Frameworks
- aiohttp
- django
- flask

## Benchmarks

- JSON Serialization: Exercises the framework fundamentals including keep-alive support, request routing, request header parsing, object instantiation, JSON serialization, response header generation, and request count throughput.

- Single Database Query: Exercises the framework's object-relational mapper (ORM), random number generator, database driver, and database connection pool.
- ?Multiple Database Queries: A variation of Test #2 and also uses the World table. Multiple rows are fetched to more dramatically punish the database driver and connection pool. At the highest queries-per-request tested (20), this test demonstrates all frameworks' convergence toward zero requests-per-second as database activity increases.
- Plaintext: An exercise of the request-routing fundamentals only, designed to demonstrate the capacity of high-performance platforms in particular. Requests will be sent using HTTP pipelining. The response payload is still small, meaning good performance is still necessary in order to saturate the gigabit Ethernet of the test environment.


# Gunicorn 

Веб-сервер, в нашем случае Nginx, принимает и обрабатывает HTTP-запрос браузера, затем передаёт его в Application-сервер — Gunicorn.
Gunicorn получает данные от Nginx, разбирает их и исходя из своей конфигурации по протоколу WSGI передаёт их в Django.
Django обрабатывает полученные данные и возвращает результат работы обратно в Gunicorn, а он в свою очередь отдаёт результат в Nginx, который возвращает клиенту готовую HTML-страницу.


# wrk - a HTTP benchmarking tool

  wrk is a modern HTTP benchmarking tool capable of generating significant
  load when run on a single multi-core CPU. It combines a multithreaded
  design with scalable event notification systems such as epoll and kqueue.

  An optional LuaJIT script can perform HTTP request generation, response
  processing, and custom reporting. Details are available in SCRIPTING and
  several examples are located in [scripts/](scripts/).

## Basic Usage

    wrk -t12 -c400 -d30s http://127.0.0.1:8080/index.html

  This runs a benchmark for 30 seconds, using 12 threads, and keeping
  400 HTTP connections open.

  Output:

    Running 30s test @ http://127.0.0.1:8080/index.html
      12 threads and 400 connections
      Thread Stats   Avg      Stdev     Max   +/- Stdev
        Latency   635.91us    0.89ms  12.92ms   93.69%
        Req/Sec    56.20k     8.07k   62.00k    86.54%
      22464657 requests in 30.00s, 17.76GB read
    Requests/sec: 748868.53
    Transfer/sec:    606.33MB

## Command Line Options

    -c, --connections: total number of HTTP connections to keep open with
                       each thread handling N = connections/threads

    -d, --duration:    duration of the test, e.g. 2s, 2m, 2h

    -t, --threads:     total number of threads to use

    -s, --script:      LuaJIT script, see SCRIPTING

    -H, --header:      HTTP header to add to request, e.g. "User-Agent: wrk"

        --latency:     print detailed latency statistics

        --timeout:     record a timeout if a response is not received within
                       this amount of time.



оба "classification": "Micro",
```buildoutcfg
{
  "rawData": {
    "plaintext": {
      "crax": [
        {
          "latencyAvg": "94.05ms",
          "latencyMax": "330.23ms",
          "latencyStdev": "54.34ms",
          "totalRequests": 385172,
          "startTime": 1668374170,
          "endTime": 1668374186
        }
      ]
    },
    "query": {},
    "json": {
      "crax": [
        {
          "latencyAvg": "1.09ms",
          "latencyMax": "33.22ms",
          "latencyStdev": "1.52ms",
          "totalRequests": 292335,
          "startTime": 1668374120,
          "endTime": 1668374135
        }
      ]
    },
    "commitCounts": {
      "crax": 0
    },
    "slocCounts": {
      "crax": 124
    }
  },
  "environmentDescription": "(unspecified, hostname = 46deb9083dc7)",
  "git": null,
  "uuid": "a56113a7-19a4-4832-8b68-e348ddc58954",
  "succeeded": {
    "plaintext": [
      "crax"
    ],
    "query": [],
    "json": [
      "crax"
    ]
  },
  "failed": {
    "plaintext": [],
    "query": [
      "crax"
    ],
    "json": []
  },
  "verify": {
    "crax": {
      "plaintext": "pass",
      "query": "fail",
      "json": "pass"
    }
  },
  "duration": 15,
  "testMetadata": [
    {
      "versus": "",
      "project_name": "aiohttp",
      "display_name": "aiohttp",
      "name": "aiohttp",
      "classification": "micro",
      "database": "postgres",
      "language": "python",
      "os": "linux",
      "notes": "uses aiopg with sqlalchemy for database access",
      "tags": [],
      "framework": "aiohttp",
      "webserver": "gunicorn",
      "orm": "full",
      "platform": "asyncio",
      "database_os": "linux",
      "approach": "realistic"
    },
    {
      "versus": "",
      "project_name": "crax",
      "display_name": "Crax",
      "name": "crax",
      "classification": "micro",
      "database": "postgres",
      "language": "python",
      "os": "linux",
      "notes": "",
      "tags": [],
      "framework": "crax",
      "webserver": "none",
      "orm": "raw",
      "platform": "none",
      "database_os": "linux",
      "approach": "realistic"
    }
  ],
  "frameworks": [
    "crax"
  ],
  "pipelineConcurrencyLevels": [
    256
  ],
  "completionTime": 1668374257009,
  "concurrencyLevels": [
    16
  ],
  "startTime": 1668373925906,
  "queryIntervals": [
    1
  ],
  "completed": {
    "crax": "20221113211628"
  },
  "name": "(unspecified, datetime = 2022-11-13 21:12:05)"
}
```
