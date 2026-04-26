# Docker

Esta pasta concentra a infraestrutura local do projeto.

## Arquivo principal

- `docker-compose.infrastructure.yml`: sobe o `SQL Server 2022 Developer` em ambiente local.
- `docker-compose.fullstack.yml`: stack completa com banco, API, Angular e React.
- `docker-compose.fullstack.template.yml`: referencia historica da stack completa.
- `Dockerfile.api`: imagem real da API `.NET 8`.
- `Dockerfile.angular`: imagem real do frontend Angular com `nginx`.
- `Dockerfile.react`: imagem real do frontend React com `nginx`.
- `.env.example`: modelo de variavel de ambiente para senha do banco.

## Preparacao

```bash
cp docker/.env.example docker/.env
```

## Uso com WSL

Padrao adotado neste projeto:

- `Docker Desktop` instalado no Windows;
- integracao com a distribuicao Ubuntu habilitada no `Docker Desktop`;
- comandos `docker` e `docker compose` executados dentro do WSL;
- acesso aos servicos pelo navegador do Windows em `localhost`.

## Regra obrigatoria

- nunca instalar `Docker Engine` dentro do WSL para esta solucao;
- nunca tratar o Ubuntu como host Docker independente;
- sempre usar o engine do `Docker Desktop` do Windows 11.

## Comando padrao

```bash
docker compose -f docker/docker-compose.infrastructure.yml up -d
```

## Stack completa

Para subir a stack completa com banco, API e os dois frontends:

```bash
./scripts/start-fullstack.sh
```

Para parar toda a stack, preservando o volume do banco:

```bash
./scripts/stop-fullstack.sh
```

Para apagar tudo da stack completa e iniciar novamente do zero:

```bash
./scripts/reset-fullstack.sh
```

Alternativa manual:

```bash
docker compose -f docker/docker-compose.fullstack.yml up -d --build
./scripts/apply-database.sh
```

## Objetivo

- padronizar o ambiente de desenvolvimento;
- evitar dependencia manual de banco instalado na maquina;
- facilitar apresentacao e onboarding do cliente;
- permitir execucao dos scripts de `database/` a partir do proprio container.

## Estrutura Docker em uso

- `sqlserver`: banco oficial do projeto;
- `api`: backend `.NET 8`;
- `frontend-angular`: aplicacao principal;
- `frontend-react`: modulo complementar.

## Observacao

O arquivo de infraestrutura continua sendo o caminho mais rapido para trabalhar apenas
com o banco.

O arquivo `docker-compose.fullstack.yml` ja sobe a stack real com os Dockerfiles do
repositorio. Os frontends consomem a API por `/api/v1` usando proxy local no
desenvolvimento e `nginx` na execucao containerizada.

Fluxo pratico recomendado:

- usar `start-fullstack.sh` para subir tudo;
- usar `stop-fullstack.sh` para parar tudo mantendo o banco;
- usar `reset-fullstack.sh` para recomecar do zero, incluindo limpeza do volume
  do SQL Server.
- usar `prepare-delivery-artifacts.sh` para consolidar os artefatos tecnicos da
  entrega apos validar a stack.

Nao e necessario instalar navegador dentro do WSL para trabalhar com a stack.

## Limpeza opcional do WSL

Caso exista instalacao local de Docker dentro do Ubuntu, remova:

```bash
sudo apt remove -y docker.io docker-doc docker-compose docker-compose-v2 podman-docker containerd runc docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo apt autoremove -y
sudo rm -rf /var/lib/docker /var/lib/containerd
```
