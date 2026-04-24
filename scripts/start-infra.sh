#!/usr/bin/env bash
set -euo pipefail

if [ ! -f "docker/.env" ]; then
  cp docker/.env.example docker/.env
fi

docker compose -f docker/docker-compose.infrastructure.yml up -d
