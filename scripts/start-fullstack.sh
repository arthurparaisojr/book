#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/lib/docker-desktop-check.sh"

if [ ! -f "docker/.env" ]; then
  cp docker/.env.example docker/.env
fi

ensure_docker_desktop_wsl_integration

wait_for_container_running() {
  local container_name="$1"
  local attempts="${2:-60}"

  for attempt in $(seq 1 "${attempts}"); do
    local state
    state="$(docker inspect --format='{{.State.Status}}' "${container_name}" 2>/dev/null || true)"

    if [ "${state}" = "running" ]; then
      return 0
    fi

    sleep 2
  done

  echo "Container ${container_name} nao ficou em execucao dentro do tempo esperado." >&2
  return 1
}

docker compose -f docker/docker-compose.fullstack.yml up -d --build

echo "Aguardando SQL Server ficar healthy..."
wait_for_container_healthy "book-sqlserver"

echo "Aplicando scripts do banco na stack fullstack..."
"$(dirname "$0")/apply-database.sh" book-sqlserver

echo "Aguardando API e frontends entrarem em execucao..."
wait_for_container_running "book-api"
wait_for_container_running "book-frontend-angular"
wait_for_container_running "book-frontend-react"

cat <<'EOF'
Stack fullstack inicializada.

Acessos locais:
- Swagger/API: http://localhost:8080/swagger
- Angular: http://localhost:4200
- React: http://localhost:4173
EOF
