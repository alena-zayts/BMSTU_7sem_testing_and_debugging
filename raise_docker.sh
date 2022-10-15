#!/bin/bash

echo "PATH_TO_PROJECT=${PWD}" > .docker-env
docker-compose --env-file .docker-env -f ${PWD}/SkiResort/tarantool/docker-compose-console.yml up -d
echo "[101;93m Docker started [0m"