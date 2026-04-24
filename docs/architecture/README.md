# Arquitetura

## Visao geral

O projeto `Book` sera estruturado como uma solucao web em camadas, separando regra de
negocio, infraestrutura, experiencia do usuario e artefatos de implantacao.

## Stack escolhida

- Backend: `.NET 8` com `ASP.NET Core Web API`
- Persistencia: `EF Core` para CRUD e `Dapper` para consultas otimizadas
- Banco: `SQL Server 2022 Developer`
- Frontend principal: `Angular`
- Frontend complementar: `React`
- Estilo: `CSS` com variaveis, responsividade e convencoes visuais
- Containers: `Docker`

## Camadas

- `Domain`: entidades como `Livro`, `Autor` e `Assunto`, incluindo validacoes centrais.
- `Application`: casos de uso, DTOs, servicos e contratos.
- `Infrastructure`: repositorios, acesso ao banco, integracoes e logs.
- `API`: controllers, filtros, middlewares, autenticacao JWT e OpenAPI.
- `Frontend Angular`: CRUD principal, formularios, listagens e navegacao.
- `Frontend React`: modulo complementar para relatorios, dashboard ou componentes
  especializados.
- `Database`: schema, views, procedures, triggers, indices e seeds.

## Distribuicao recomendada

```text
src/backend/
|-- Book.Api
|-- Book.Application
|-- Book.Domain
`-- Book.Infrastructure

src/frontend-angular/
`-- book-admin

src/frontend-react/
`-- book-insights
```

## Diretrizes arquiteturais

- O Angular sera a interface principal do sistema.
- O React sera usado para um modulo complementar, sem duplicar o CRUD principal.
- A API deve expor contratos claros, com versionamento e documentacao.
- A autenticacao padrao sera `JWT Bearer`.
- O banco deve continuar como fonte oficial de relatorio e integridade relacional.
- Excecoes devem ser centralizadas em middleware, nao espalhadas pelos controllers.
- O idioma padrao sera `pt-BR` em telas, mensagens e mascaras.
