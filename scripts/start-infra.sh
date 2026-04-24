#!/usr/bin/env bash
set -euo pipefail

if [ ! -f "docker/.env" ]; then
  cp docker/.env.example docker/.env
fi

set -a
# shellcheck disable=SC1091
source docker/.env
set +a

docker compose -f docker/docker-compose.infrastructure.yml up -d

echo "Aguardando SQL Server ficar pronto..."

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
