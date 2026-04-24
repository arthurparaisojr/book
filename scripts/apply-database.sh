#!/usr/bin/env bash
set -euo pipefail

container_name="${1:-book-sqlserver}"
password="${BOOK_SQL_SA_PASSWORD:-Book@123456}"

run_sql() {
  local file="$1"
  echo "Executando ${file}"
  docker exec -i "${container_name}" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "${password}" -C \
    -i "/workspace/${file}"
}

run_sql "database/schema/001_create_base_tables.sql"
run_sql "database/schema/002_create_indexes_and_constraints.sql"
run_sql "database/schema/003_create_audit_tables.sql"
run_sql "database/views/001_vw_relatorio_livros_por_autor.sql"
run_sql "database/procedures/001_pr_livro_obter_por_filtros.sql"
run_sql "database/procedures/002_pr_relatorio_livros_por_autor.sql"
run_sql "database/triggers/001_trg_livro_audit.sql"
run_sql "database/seeds/001_seed_initial_data.sql"
