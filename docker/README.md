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
