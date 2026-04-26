#!/usr/bin/env bash
set -euo pipefail

api_base_url="${1:-http://localhost:5268}"
output_file="${2:-artifacts/api/book-api-openapi-v1.json}"
swagger_url="${api_base_url%/}/swagger/v1/swagger.json"
api_pid=""

cleanup() {
  if [ -n "${api_pid}" ] && kill -0 "${api_pid}" >/dev/null 2>&1; then
    kill "${api_pid}" >/dev/null 2>&1 || true
    wait "${api_pid}" >/dev/null 2>&1 || true
  fi
}

trap cleanup EXIT

wait_for_swagger() {
  local url="$1"

  for attempt in $(seq 1 60); do
    if curl --silent --fail "${url}" >/dev/null 2>&1; then
      return 0
    fi

    sleep 2
  done

  return 1
}

mkdir -p "$(dirname "${output_file}")"

if ! curl --silent --fail "${swagger_url}" >/dev/null 2>&1; then
  echo "Swagger nao estava acessivel em ${swagger_url}. Subindo a API localmente..."

  (
    cd src/backend/Book.Api
    dotnet run --launch-profile http
  ) >/tmp/book-api-openapi.log 2>&1 &
  api_pid="$!"

  if ! wait_for_swagger "${swagger_url}"; then
    echo "Nao foi possivel disponibilizar o Swagger em ${swagger_url}." >&2
    echo "Consulte /tmp/book-api-openapi.log para diagnostico." >&2
    exit 1
  fi
fi

curl --silent --fail "${swagger_url}" -o "${output_file}"

echo "OpenAPI exportado com sucesso para ${output_file}."
