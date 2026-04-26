#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/lib/docker-desktop-check.sh"

if [ ! -f "docker/.env" ]; then
  cp docker/.env.example docker/.env
fi

ensure_docker_desktop_wsl_integration

docker compose -f docker/docker-compose.fullstack.yml down -v --remove-orphans

echo "Volume do SQL Server removido. Iniciando stack limpa..."
"$(dirname "$0")/start-fullstack.sh"
