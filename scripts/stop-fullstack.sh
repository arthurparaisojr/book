#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/lib/docker-desktop-check.sh"

ensure_docker_desktop_wsl_integration

docker compose -f docker/docker-compose.fullstack.yml down --remove-orphans

cat <<'EOF'
Stack fullstack parada com sucesso.

Foram removidos os containers e redes da stack.
Os volumes do banco foram preservados.
EOF
