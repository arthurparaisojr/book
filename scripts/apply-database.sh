#!/usr/bin/env bash
set -euo pipefail

if [ ! -f "docker/.env" ]; then
  cp docker/.env.example docker/.env
fi

set -a
# shellcheck disable=SC1091
source docker/.env
set +a

container_name="${1:-book-sqlserver}"
password="${BOOK_SQL_SA_PASSWORD}"

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
  return 1
}

run_sql() {
  local file="$1"
  echo "Executando ${file}"
  docker exec -i "${container_name}" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "${password}" -C -b \
    -i "/workspace/${file}"
}

wait_for_sqlserver

run_sql "database/schema/001_create_base_tables.sql"
run_sql "database/schema/002_create_indexes_and_constraints.sql"
run_sql "database/schema/003_create_audit_tables.sql"
run_sql "database/views/001_vw_relatorio_livros_por_autor.sql"
run_sql "database/procedures/001_pr_livro_obter_por_filtros.sql"
run_sql "database/procedures/002_pr_relatorio_livros_por_autor.sql"
run_sql "database/triggers/001_trg_livro_audit.sql"
run_sql "database/seeds/001_seed_initial_data.sql"
