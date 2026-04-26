#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/lib/docker-desktop-check.sh"

if [ ! -f "docker/.env" ]; then
  cp docker/.env.example docker/.env
fi

set -a
# shellcheck disable=SC1091
source docker/.env
set +a

ensure_docker_desktop_wsl_integration

docker compose -f docker/docker-compose.infrastructure.yml up -d

echo "Aguardando container do SQL Server ficar healthy..."

wait_for_container_healthy "book-sqlserver"

echo "Aguardando SQL Server aceitar conexoes..."

for attempt in $(seq 1 60); do
  if docker exec -i book-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "${BOOK_SQL_SA_PASSWORD}" -C \
    -Q "SELECT 1" >/dev/null 2>&1; then
    echo "SQL Server pronto."
    exit 0
  fi

  sleep 2
done

echo "SQL Server nao ficou pronto dentro do tempo esperado." >&2
exit 1
