#!/bin/bash
echo hello
docker-compose up -d

sleep 2

docker-compose exec mongo1 sh init.sh

$SHELL