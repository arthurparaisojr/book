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
- Estilo: `CSS` com variaveis, responsividade, biblioteca de temas e convencoes
  visuais em azuis
- Containers: `Docker`

## Por que usar Angular e React

Mesmo sendo uma exigencia do desafio, a coexistencia dos dois frontends precisa ser
tecnicamente justificavel.

- `Angular` foi escolhido para a operacao principal porque o projeto pede
  autenticacao, CRUD, formularios, validacoes, navegacao administrativa e uma
  estrutura de aplicacao forte para backoffice.
- `React` foi escolhido para o modulo complementar porque oferece boa flexibilidade
  para dashboards, leitura analitica, vitrine tecnica e composicao de interfaces
  orientadas a visualizacao.

Diretriz de arquitetura:

- `Angular` concentra a operacao;
- `React` concentra leitura e analise;
- os dois compartilham o mesmo tema, favicon e biblioteca de icones em `src/shared/`.

## Camadas

- `Domain`: entidades como `Livro`, `Autor` e `Assunto`, incluindo validacoes centrais.
- `Application`: casos de uso, DTOs, servicos e contratos.
- `Infrastructure`: repositorios, acesso ao banco, integracoes e logs.
- `API`: controllers, filtros, middlewares, autenticacao JWT e OpenAPI.
- `Frontend Angular`: CRUD principal, formularios, listagens e navegacao.
- `Frontend React`: modulo complementar para relatorios, dashboard ou componentes
  especializados.
- `Shared UI`: temas CSS, design tokens e biblioteca de icones SVG.
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

src/shared/
|-- icons/
|   `-- svg/
`-- themes/
```

## Diretrizes arquiteturais

- O Angular sera a interface principal do sistema.
- O React sera usado para um modulo complementar, sem duplicar o CRUD principal.
- O Angular e o React devem compartilhar tema, tokens visuais, favicon e icones.
- A API deve expor contratos claros, com versionamento e documentacao.
- A autenticacao padrao sera `JWT Bearer`.
- O banco deve continuar como fonte oficial de relatorio e integridade relacional.
- Excecoes devem ser centralizadas em middleware, nao espalhadas pelos controllers.
- O idioma padrao sera `pt-BR` em telas, mensagens e mascaras.
- O frontend deve manter uma experiencia amigavel e visual consistente.
- O tema padrao do projeto deve usar azuis como base visual.
- Os icones SVG devem ser compartilhados a partir de `src/shared/icons/svg/`.
