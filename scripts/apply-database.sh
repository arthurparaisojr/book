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

container_name="${1:-book-sqlserver}"
password="${BOOK_SQL_SA_PASSWORD}"

ensure_container_exists() {
  if ! docker container inspect "${container_name}" >/dev/null 2>&1; then
    echo "Container ${container_name} nao encontrado. Execute ./scripts/start-infra.sh antes de aplicar o banco." >&2
    exit 1
  fi
}

wait_for_container_health() {
  echo "Aguardando container ${container_name} ficar healthy..."
  wait_for_container_healthy "${container_name}"
}

wait_for_sqlserver() {
  echo "Validando conexao com SQL Server..."

  for attempt in $(seq 1 60); do
    if docker exec -i "${container_name}" /opt/mssql-tools18/bin/sqlcmd \
      -S localhost -U sa -P "${password}" -C \
      -Q "SELECT 1" >/dev/null 2>&1; then
      echo "Conexao com SQL Server validada."
      return 0
    fi

    sleep 2
  done

  echo "Nao foi possivel conectar ao SQL Server no container ${container_name}." >&2
  echo "Confirme se o Docker Desktop esta integrado ao WSL e se o container esta ativo." >&2
  return 1
}

run_sql() {
  local file="$1"
  local attempt

  for attempt in $(seq 1 3); do
    echo "Executando ${file} (tentativa ${attempt}/3)"

    if docker exec -i "${container_name}" /opt/mssql-tools18/bin/sqlcmd \
      -S localhost -U sa -P "${password}" -C -b \
      -i "/workspace/${file}"; then
      return 0
    fi

    if [ "${attempt}" -lt 3 ]; then
      echo "Falha transitoria ao executar ${file}. Aguardando antes de tentar novamente..." >&2
      sleep 2
    fi
  done

  echo "Falha definitiva ao executar ${file} apos 3 tentativas." >&2
  return 1
}

ensure_docker_desktop_wsl_integration
ensure_container_exists
wait_for_container_health
wait_for_sqlserver

run_sql "database/schema/001_create_base_tables.sql"
run_sql "database/schema/002_create_indexes_and_constraints.sql"
run_sql "database/schema/003_create_audit_tables.sql"
run_sql "database/views/001_vw_relatorio_livros_por_autor.sql"
run_sql "database/procedures/001_pr_livro_obter_por_filtros.sql"
run_sql "database/procedures/002_pr_relatorio_livros_por_autor.sql"
run_sql "database/triggers/001_trg_livro_audit.sql"
run_sql "database/seeds/001_seed_initial_data.sql"
