# Docker

Esta pasta concentra a infraestrutura local do projeto.

## Arquivo principal

- `docker-compose.infrastructure.yml`: sobe o `SQL Server 2022 Developer` em ambiente local.
- `docker-compose.fullstack.template.yml`: template da stack completa com banco, API,
  Angular e React.
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

## Objetivo

- padronizar o ambiente de desenvolvimento;
- evitar dependencia manual de banco instalado na maquina;
- facilitar apresentacao e onboarding do cliente;
- permitir execucao dos scripts de `database/` a partir do proprio container.

## Estrutura Docker prevista

- `sqlserver`: banco oficial do projeto;
- `api`: backend `.NET 8`;
- `frontend-angular`: aplicacao principal;
- `frontend-react`: modulo complementar.

## Observacao

O arquivo de infraestrutura atual ja sobe o banco.

O arquivo `docker-compose.fullstack.template.yml` foi incluido como estrutura de
referencia para a stack completa e deve ser usado quando os Dockerfiles e aplicacoes
estiverem prontos.

Nao e necessario instalar navegador dentro do WSL para trabalhar com a stack.

## Limpeza opcional do WSL

Caso exista instalacao local de Docker dentro do Ubuntu, remova:

```bash
sudo apt remove -y docker.io docker-doc docker-compose docker-compose-v2 podman-docker containerd runc docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo apt autoremove -y
sudo rm -rf /var/lib/docker /var/lib/containerd
```
