# Roteiro de Apresentacao Tecnica

## Objetivo

Orientar a demonstracao final do projeto `Book` na etapa `6.B`, cobrindo arquitetura,
stack, banco, API, frontends e operacao via Docker.

## Ordem sugerida da apresentacao

1. Contexto do projeto
2. Arquitetura e stack oficial
3. Banco de dados e relatorio baseado em `view`
4. API `.NET 8` com `JWT Bearer`, `ProblemDetails` e health checks
5. Frontend Angular para operacao principal
6. Frontend React para leitura analitica e relatorio
7. Orquestracao Docker e scripts do projeto
8. Artefatos tecnicos da entrega

## 1. Contexto do projeto

Apresentar:

- objetivo do projeto `Book`;
- escopo com CRUD de `Livro`, `Autor` e `Assunto`;
- campo `Valor` com regra monetaria em `pt-BR`;
- relatorio por autor a partir da `view` oficial do banco.

## 2. Arquitetura e stack

Apresentar:

- backend em `.NET 8`;
- autenticacao `JWT Bearer`;
- frontend principal em `Angular`;
- frontend complementar em `React`;
- tema compartilhado em `src/shared/themes/`;
- icones compartilhados em `src/shared/icons/svg/`;
- banco `SQL Server 2022 Developer` via `Docker Desktop`.

Referencias:

- `README.md`
- `docs/architecture/README.md`
- `docs/security/README.md`

## 3. Banco de dados

Mostrar:

- scripts versionados em `database/`;
- tabelas principais e N:N;
- campo `Valor` como `DECIMAL(10,2)`;
- `view` `dbo.vw_RelatorioLivrosPorAutor`;
- `procedures` e `trigger` de auditoria;
- evidencias em `artifacts/database/`.

Referencias:

- `artifacts/database/relatorio-detalhado-banco.md`
- `artifacts/database/checklist-validacao-banco.md`
- `artifacts/database/validacao-banco-output.txt`

## 4. API

Demonstrar:

- `Swagger` em `http://localhost:8080/swagger` na stack Docker;
- login em `POST /api/v1/auth/login`;
- health checks;
- CRUDs e relatorio por autor;
- padrao de erro com `ProblemDetails`;
- export `OpenAPI` em `artifacts/api/book-api-openapi-v1.json`.

## 5. Frontend Angular

Demonstrar:

- login com `book-admin` e `book-reader`;
- CRUD administrativo;
- mascara monetaria em `pt-BR`;
- mensagens amigaveis de validacao;
- navegacao acessivel e tema azul compartilhado.

URL:

- `http://localhost:4200`

## 6. Frontend React

Demonstrar:

- dashboard de leitura;
- indicadores de saude da API;
- busca local;
- relatorio por autor consumindo `/api/v1/relatorios/livros-por-autor`;
- consistencia visual com o Angular.

URL:

- `http://localhost:4173`

## 7. Docker e scripts

Mostrar:

- `./scripts/start-fullstack.sh` para subir tudo;
- `./scripts/stop-fullstack.sh` para parar tudo;
- `./scripts/reset-fullstack.sh` para recomecar do zero;
- `./scripts/prepare-delivery-artifacts.sh` para consolidar artefatos.

## 8. Fechamento

Encerrar reforcando:

- stack padronizada e documentada;
- reproducao local do zero;
- scripts de operacao e reset;
- artefatos de API, banco e apresentacao prontos para entrega.
