#!/usr/bin/env bash

git clone https://github.com/jsusmachaca/KafkaDotnet.git /tmp/KafkaDotnet

echo -e "KAFKA_SERVER=$KAFKA_SERVER\n" > .env

docker compose -f /tmp/KafkaDotnet/docker-compose.yml up -d
