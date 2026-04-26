#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/lib/docker-desktop-check.sh"

ensure_docker_desktop_wsl_integration

echo "Exportando OpenAPI..."
"$(dirname "$0")/export-openapi.sh"

if docker container inspect book-sqlserver >/dev/null 2>&1; then
  echo "Executando validacao do banco para apoiar a entrega tecnica..."
  {
    echo "# Validacao do Banco - $(date -u +"%Y-%m-%dT%H:%M:%SZ")"
    echo
    "$(dirname "$0")/validate-database.sh"
  } > artifacts/database/validacao-banco-output.txt
  echo "Saida da validacao gravada em artifacts/database/validacao-banco-output.txt."
else
  echo "Container book-sqlserver nao encontrado. A validacao do banco foi pulada."
fi

cat <<'EOF'
Artefatos tecnicos preparados.

Arquivos principais:
- artifacts/api/book-api-openapi-v1.json
- artifacts/database/validacao-banco-output.txt
- artifacts/reports/roteiro-apresentacao-tecnica.md
- artifacts/reports/checklist-entrega-6b.md
EOF
