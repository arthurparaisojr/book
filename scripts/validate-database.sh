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
  for attempt in $(seq 1 60); do
    if docker exec -i "${container_name}" /opt/mssql-tools18/bin/sqlcmd \
      -S localhost -U sa -P "${password}" -C \
      -Q "SELECT 1" >/dev/null 2>&1; then
      return 0
    fi

    sleep 2
  done

  echo "Nao foi possivel conectar ao SQL Server no container ${container_name}." >&2
  return 1
}

run_query() {
  local label="$1"
  local query="$2"

  echo
  echo "==> ${label}"
  docker exec -i "${container_name}" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "${password}" -C -b -y 120 -Y 120 \
    -d BookDb \
    -Q "${query}"
}

wait_for_sqlserver

run_query \
  "Contagem das tabelas principais" \
  "SELECT 'Autor' AS Tabela, COUNT(*) AS Quantidade FROM dbo.Autor
   UNION ALL
   SELECT 'Assunto', COUNT(*) FROM dbo.Assunto
   UNION ALL
   SELECT 'Livro', COUNT(*) FROM dbo.Livro
   UNION ALL
   SELECT 'Livro_Autor', COUNT(*) FROM dbo.Livro_Autor
   UNION ALL
   SELECT 'Livro_Assunto', COUNT(*) FROM dbo.Livro_Assunto;"

run_query \
  "Amostra da view de relatorio" \
  "SELECT TOP 5 * FROM dbo.vw_RelatorioLivrosPorAutor ORDER BY AutorNome, Titulo;"

run_query \
  "Procedure de filtro de livros" \
  "EXEC dbo.pr_Livro_ObterPorFiltros @Titulo = 'Clean';"

run_query \
  "Procedure de relatorio por autor" \
  "EXEC dbo.pr_RelatorioLivrosPorAutor @AutorNome = 'Fowler';"

run_query \
  "Validacao da trigger em transacao controlada" \
  "BEGIN TRAN;
   DECLARE @auditAntes INT = (SELECT COUNT(*) FROM dbo.Livro_Audit);
   UPDATE dbo.Livro
   SET Valor = Valor + 1
   WHERE Codl = 1;
   SELECT @auditAntes AS AuditAntes, COUNT(*) AS AuditDepois FROM dbo.Livro_Audit;
   ROLLBACK TRAN;"

echo
echo "Validacao do banco concluida com sucesso."
