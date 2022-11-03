#!/bin/bash

echo "PATH_TO_PROJECT=${PWD}" > .docker-env
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml down
echo "[101;93m Docker stopped [0m"