#!/usr/bin/env bash

ensure_docker_desktop_wsl_integration() {
  if ! command -v docker >/dev/null 2>&1; then
    cat >&2 <<'EOF'
Docker nao esta disponivel nesta distro WSL.

Este projeto usa exclusivamente o Docker Desktop do Windows 11 com integracao WSL.

Como corrigir:
1. abrir o Docker Desktop no Windows;
2. ir em Settings > General e manter ativo "Use the WSL 2 based engine";
3. ir em Settings > Resources > WSL Integration;
4. habilitar a distro Ubuntu usada neste projeto;
5. clicar em Apply & Restart;
6. no PowerShell, executar: wsl --shutdown;
7. abrir o WSL novamente e validar com:
   docker version
   docker compose version

Nao instale Docker Engine dentro do WSL para esta solucao.
EOF
    exit 1
  fi

  if ! docker version >/dev/null 2>&1; then
    cat >&2 <<'EOF'
O comando docker existe no WSL, mas nao conseguiu falar com o Docker Desktop.

Verifique:
1. se o Docker Desktop esta aberto no Windows;
2. se a integracao WSL desta distro esta habilitada;
3. se o comando "docker version" funciona no WSL antes de rodar os scripts.
EOF
    exit 1
  fi
}

wait_for_container_healthy() {
  local container_name="$1"
  local attempts="${2:-60}"

  for attempt in $(seq 1 "${attempts}"); do
    local health_status
    health_status="$(docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}no-healthcheck{{end}}' "${container_name}" 2>/dev/null || true)"

    if [ "${health_status}" = "healthy" ] || [ "${health_status}" = "no-healthcheck" ]; then
      return 0
    fi

    sleep 2
  done

  echo "Container ${container_name} nao atingiu estado healthy dentro do tempo esperado." >&2
  return 1
}
