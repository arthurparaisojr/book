# Book

Projeto base para um sistema web de cadastro de livros, autores e assuntos, organizado
para demonstrar arquitetura moderna, boa documentação, banco de dados relacional bem
desenhado, API bem escrita, uso de Docker, tratamento de exceções e processo de
entrega profissional.

## 1. Objetivo

Criar o projeto `Book` seguindo boas práticas de mercado e deixando todo o passo a
passo documentado para que o cliente consiga:

- entender a arquitetura;
- subir a solução do zero;
- acompanhar backlog, camadas e artefatos;
- executar banco, API e frontends;
- apresentar tecnicamente a solução com segurança.

## 2. Escopo funcional

O projeto consiste em um cadastro de livros com:

- CRUD de `Livro`;
- CRUD de `Autor`;
- CRUD de `Assunto`;
- relacionamentos N:N entre livro/autor e livro/assunto;
- campo adicional `Valor` em `Livro`, com máscara monetária em `pt-BR`;
- relatório baseado em `view` do banco, agrupado por autor;
- tratamento de erros específicos, evitando `try/catch` genérico;
- documentação de implantação e apresentação.

## 3. Stack oficial do projeto

As tecnologias definidas para o projeto são:

- Backend: `.NET 8` com `ASP.NET Core Web API`;
- Autenticação: `JWT Bearer` na API, com possibilidade de evolução para refresh token;
- Frontend principal: `Angular` para o CRUD administrativo;
- Frontend complementar: `React` para módulo de apoio analítico, relatórios ou vitrine
  técnica de componentes;
- Estilo: `CSS` com variáveis, responsividade, biblioteca de temas e padrão visual em
  azuis amigáveis;
- Banco de dados: `SQL Server 2022 Developer`, com execução local via `Docker`;
- Persistência: `EF Core` para fluxo transacional e `Dapper` para consultas de leitura
  e relatório quando fizer sentido;
- Documentação da API: `Swagger / OpenAPI`;
- Testes: `xUnit`, `FluentAssertions`, `Jest`, `Vitest` e `Playwright`;
- Observabilidade: `Serilog` com logs estruturados.

### 3.1 Justificativa para Angular e React no mesmo projeto

Mesmo tendo sido uma exigencia do desafio, o uso conjunto de `Angular` e `React`
precisa ter criterio arquitetural claro.

Decisao adotada:

- `Angular` foi definido como frontend principal porque o projeto exige CRUD
  administrativo, formularios robustos, validacoes, autenticacao, roteamento e
  manutencao de um painel operacional consistente.
- `React` foi mantido como frontend complementar porque funciona muito bem para
  dashboard, leitura analitica, apresentacao executiva e composicao de experiencias
  mais livres, voltadas a visualizacao.

Em resumo:

- `Angular` = operacao do sistema;
- `React` = leitura e analise complementar.

Assim, a arquitetura demonstra dominio das duas tecnologias sem duplicar
responsabilidades por modismo. Cada framework foi posicionado onde entrega mais
clareza, produtividade e valor.

## 4. Arquitetura e camadas

O projeto deverá ser organizado nas seguintes camadas:

- Apresentação Angular: interface principal do sistema e formulários de CRUD;
- Apresentação React: módulo complementar para relatórios, dashboards ou POC técnica;
- API `.NET`: controllers, versionamento, autenticação, validações e contratos;
- Aplicação: casos de uso, orquestração, DTOs e regras de entrada;
- Domínio: entidades, regras de negócio, contratos e validações centrais;
- Infraestrutura: acesso a dados, logging, integração e persistência;
- Compartilhado: biblioteca de temas CSS, design tokens e biblioteca de ícones SVG;
- Banco de dados: tabelas, views, procedures, triggers, índices e seeds;
- Artefatos: diagramas, exports da API, relatórios e evidências de apresentação.

Detalhes adicionais estão em [`docs/architecture/README.md`](docs/architecture/README.md).

## 5. Estrutura do repositório

```text
.
|-- .cursor/
|   `-- rules/
|-- artifacts/
|   |-- api/
|   |-- database/
|   |-- diagrams/
|   `-- reports/
|-- database/
|   |-- procedures/
|   |-- schema/
|   |-- seeds/
|   |-- triggers/
|   `-- views/
|-- docker/
|-- docs/
|   |-- api/
|   |-- architecture/
|   |-- assets/
|   |   `-- images/
|   |-- backlog/
|   |-- database/
|   |-- process/
|   `-- setup/
|-- scripts/
|-- src/
|   |-- backend/
|   |-- frontend-angular/
|   |-- frontend-react/
|   `-- shared/
|       |-- icons/
|       `-- themes/
`-- tests/
    |-- backend/
    |-- frontend-angular/
    |-- frontend-react/
    `-- integration/
```

## 6. Onde colocar o modelo de dados

A imagem anexada do modelo de dados deve ser salva em:

`docs/assets/images/modelo-dados-book.png`

Esse deve ser o caminho oficial para referência nos documentos. Se for necessário
gerar versões para apresentação, PDF ou export, use `artifacts/diagrams/`.

## 6.1 Onde colocar ícones SVG

Toda biblioteca de ícones do projeto deve ser centralizada em:

`src/shared/icons/svg/`

Regra de trabalho:

- se um novo ícone SVG for necessário, o nome do arquivo deve ser definido antes;
- o local oficial para gravação deve sempre ser informado junto com o nome;
- o padrão de nomenclatura deve refletir a finalidade do ícone.

Exemplos:

- `book-menu-livros.svg`
- `book-action-salvar.svg`
- `book-report-autor.svg`

## 7. Etapas oficiais do projeto

Toda evolução do projeto deve seguir etapas numeradas e subetapas por letra:

- `1.A` Alinhar escopo, stack, convenções e documentação inicial.
- `1.B` Preparar repositório, template de commit, Git e regras de trabalho.
- `2.A` Modelar banco de dados lógico e físico.
- `2.B` Criar scripts de schema, views, procedures, triggers e seeds.
- `2.C` Validar relatório detalhado do banco e evidências.
- `3.A` Criar solução `.NET`, camadas e contratos.
- `3.B` Implementar API REST, documentação OpenAPI e tratamento de exceções.
- `3.C` Implementar autenticação, logging, health checks e observabilidade.
- `4.A` Criar frontend Angular para operação principal.
- `4.B` Criar frontend React para apoio analítico, dashboard ou módulo de relatório.
- `4.C` Definir CSS, acessibilidade, idioma e máscaras.
- `5.A` Implementar relatório baseado em `view` do banco.
- `5.B` Cobrir fluxos com testes unitários, integração e ponta a ponta.
- `6.A` Orquestrar ambiente com Docker.
- `6.B` Consolidar scripts, apresentação técnica e artefatos.
- `6.C` Validar entrega final, backlog executado e documentação.

O backlog detalhado está em [`docs/backlog/README.md`](docs/backlog/README.md).

## 7.1 Evolucoes futuras fora do escopo atual

Sem alterar o plano oficial `1.A` a `6.C`, o projeto ja registra a seguinte
evolucao arquitetural desejada para fase posterior:

- autenticacao compartilhada entre `Angular` e `React`, evitando novo login ao
  alternar entre os dois frontends;
- manutencao do `React` como modulo existente e ativo, mesmo com experiencia mais
  integrada ao restante da solucao;
- preferencia futura por sessao compartilhada no backend, idealmente com cookie
  seguro, em vez de duplicar fluxo de login em cada frontend.

Essa evolucao deve ser tratada como proxima fase de arquitetura, e nao como mudanca
do backlog atual.

## 8. Convenção obrigatória de commits

Toda alteração deve ser registrada com mensagem padronizada:

```text
tipo(etapa): descricao curta
```

Exemplos:

- `docs(1.A): organiza estrutura base do projeto Book`
- `chore(1.B): adiciona template de commit e diretrizes do cursor`
- `feat(2.B): cria view de relatorio por autor`
- `feat(3.B): implementa endpoint de livros`
- `fix(4.C): corrige mascara monetaria em pt-BR`

Tipos recomendados:

- `docs`
- `chore`
- `feat`
- `fix`
- `refactor`
- `test`
- `perf`
- `build`

O template está em [`.gitmessage`](.gitmessage) e as regras de fluxo em
[`docs/process/README.md`](docs/process/README.md).

## 9. Como criar tudo do zero

O passo a passo completo está em [`docs/setup/README.md`](docs/setup/README.md), mas a
sequência oficial é:

### 9.1 Pré-requisitos

Instale:

- `Git`
- `Docker Desktop`
- `.NET SDK 8`
- `Node.js 20 LTS`
- `npm` ou `pnpm`
- `Angular CLI`
- `SQL Server Management Studio` ou `Azure Data Studio`

Observacao para ambiente `Windows + WSL`:

- o `Docker Desktop` fica instalado no Windows;
- o WSL apenas consome o Docker com integracao habilitada;
- nao e necessario instalar navegador dentro do WSL;
- Angular, React e Swagger podem ser acessados pelo navegador do Windows via
  `http://localhost`.

Diretriz obrigatoria:

- este projeto deve usar sempre o `Docker Desktop` do `Windows 11`;
- nao deve haver instalacao de `Docker Engine` dentro do WSL para esta solucao.

### 9.2 Inicialização do repositório

```bash
git clone <url-do-repositorio> book
cd book
git config commit.template .gitmessage
cp docker/.env.example docker/.env
```

### 9.3 Banco via Docker

```bash
docker compose -f docker/docker-compose.infrastructure.yml up -d
```

Esse comando e executado no WSL, mas usa o `Docker Desktop` do Windows quando a
integracao WSL estiver habilitada.

Banco oficial do projeto:

- `SQL Server 2022 Developer`

Motivos da escolha:

- integra muito bem com `.NET 8`;
- suporta `views`, `procedures`, `triggers` e constraints com maturidade;
- funciona bem em ambiente local com `Docker`;
- atende muito bem ao perfil relacional do projeto;
- e e gratuito para desenvolvimento e testes.

Observacao:

- `SQL Server Developer` e gratuito para desenvolvimento e testes;
- se o projeto evoluir para um ambiente produtivo com exigencia de banco livre, a
  decisao deve ser reavaliada.

### 9.4 Criar a solução backend

```bash
dotnet new sln -n Book
dotnet new webapi -n Book.Api -o src/backend/Book.Api
dotnet new classlib -n Book.Application -o src/backend/Book.Application
dotnet new classlib -n Book.Domain -o src/backend/Book.Domain
dotnet new classlib -n Book.Infrastructure -o src/backend/Book.Infrastructure
dotnet new xunit -n Book.Api.Tests -o tests/backend/Book.Api.Tests
dotnet sln add src/backend/Book.Api/Book.Api.csproj
dotnet sln add src/backend/Book.Application/Book.Application.csproj
dotnet sln add src/backend/Book.Domain/Book.Domain.csproj
dotnet sln add src/backend/Book.Infrastructure/Book.Infrastructure.csproj
dotnet sln add tests/backend/Book.Api.Tests/Book.Api.Tests.csproj
```

### 9.5 Criar o frontend Angular

```bash
npx @angular/cli@latest new src/frontend-angular/book-admin --routing --style css
```

### 9.6 Criar o frontend React

```bash
npm create vite@latest src/frontend-react/book-insights -- --template react-ts
```

### 9.7 Ajustar idioma e máscaras

- Backend com cultura `pt-BR`;
- Angular com `LOCALE_ID = pt-BR`;
- React com `Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })`;
- valor persistido em banco como `DECIMAL(10,2)`;
- input monetário com máscara no front.

Implementacao atual da etapa `4.C`:

- Angular com mascara monetaria em `pt-BR`, mensagens de validacao e acessibilidade basica;
- React com skip link, busca com ajuda textual e favicon compartilhado;
- tema azul compartilhado aplicado nos dois frontends.

### 9.8 Criar banco e objetos

Executar, nesta ordem:

1. `database/schema`
2. `database/views`
3. `database/procedures`
4. `database/triggers`
5. `database/seeds`

Exemplo de execucao do primeiro script pelo container:

```bash
docker exec -it book-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Book@123456' -C \
  -i /workspace/database/schema/001_create_base_tables.sql
```

### 9.9 Implementar a API

Boas práticas mínimas:

- rotas REST versionadas;
- DTOs de entrada e saída;
- validações;
- paginação e filtros;
- autenticação `JWT Bearer`;
- autorização por perfil ou policy quando aplicável;
- `ProblemDetails` para erros;
- Swagger atualizado;
- logs estruturados;
- endpoints documentados.

### 9.10 Configurar autenticação

Padrão oficial do projeto:

- autenticação via `JWT Bearer`;
- emissão de token na API `.NET`;
- proteção de endpoints sensíveis com `[Authorize]`;
- uso futuro de `refresh token` apenas se o escopo crescer.

Para desenvolvimento local, a API sobe com credenciais de teste em
`src/backend/Book.Api/appsettings.Development.json`.

Credenciais locais:

- usuário `book-admin` com senha `Book@123`
- usuário `book-reader` com senha `Book@123`

Fluxo mínimo de teste da autenticação:

```bash
curl -X POST http://localhost:5268/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"book-admin","password":"Book@123"}'
```

Com o token retornado:

- endpoints `GET` podem ser testados sem autenticação;
- endpoints de `POST`, `PUT` e `DELETE` exigem token JWT;
- perfil `Admin` pode gravar;
- perfil `Reader` deve receber `403 Forbidden` em operações de escrita.

### 9.11 Estrutura Docker completa

Estrutura prevista para conter toda a solução:

- `docker/docker-compose.infrastructure.yml`: banco local e dependências técnicas;
- `docker/docker-compose.fullstack.yml`: stack completa com `sqlserver`, `api`,
  `frontend-angular` e `frontend-react`;
- `docker/.env.example`: variáveis base do ambiente.

Fluxo recomendado:

```bash
./scripts/start-fullstack.sh
```

Para parar tudo sem apagar o banco:

```bash
./scripts/stop-fullstack.sh
```

Para apagar toda a stack e iniciar novamente do zero:

```bash
./scripts/reset-fullstack.sh
```

Para consolidar os artefatos tecnicos da entrega:

```bash
./scripts/prepare-delivery-artifacts.sh
```

Alternativa manual:

```bash
docker compose -f docker/docker-compose.fullstack.yml up -d --build
./scripts/apply-database.sh
```

Implementacao atual da etapa `6.A`:

- compose fullstack real versionado em `docker/docker-compose.fullstack.yml`;
- Dockerfiles reais para API, Angular e React;
- proxy de `/api/v1` no Angular e no React para funcionar no dev local e no Docker;
- frontends publicados por `nginx` nas portas `4200` e `4173`;
- API exposta em `http://localhost:8080`.

Implementacao atual da etapa `6.B`:

- scripts para exportar `OpenAPI` e consolidar artefatos da entrega;
- roteiro tecnico de apresentacao em `artifacts/reports/roteiro-apresentacao-tecnica.md`;
- checklist operacional da etapa em `artifacts/reports/checklist-entrega-6b.md`;
- pasta `artifacts/api/` preparada para versionar o export oficial da API.

### 9.12 Construir relatórios

O relatório deve vir de uma `view` do banco e agrupar dados por autor. Os modelos e
orientações estão em `database/views`, `docs/database` e `artifacts/reports`.

## 10. Diretrizes não negociáveis

- O modelo de dados deve ser seguido integralmente, salvo ajustes controlados para
  performance.
- O campo `Valor` é obrigatório e deve usar máscara monetária.
- Toda exceção deve ser tratada de forma específica, com log e retorno coerente.
- A API deve ser bem escrita, documentada e previsível.
- O backlog deve refletir camadas e etapas do projeto.
- O frontend deve ser amigável, claro e usar tema padrão em azuis.
- A biblioteca de temas CSS deve ficar em `src/shared/themes/`.
- A biblioteca de ícones SVG deve ficar em `src/shared/icons/svg/`.
- Views, procedures, triggers e scripts de implantação devem ficar versionados.
- Toda alteração relevante exige atualização de documentação e commit padronizado.
- O idioma padrão da solução deve ser `pt-BR`, salvo necessidade técnica justificada.

## 11. Documentos complementares

- [`docs/README.md`](docs/README.md)
- [`docs/architecture/README.md`](docs/architecture/README.md)
- [`docs/api/README.md`](docs/api/README.md)
- [`docs/database/README.md`](docs/database/README.md)
- [`docs/security/README.md`](docs/security/README.md)
- [`docs/backlog/README.md`](docs/backlog/README.md)
- [`docs/process/README.md`](docs/process/README.md)
- [`docs/setup/README.md`](docs/setup/README.md)
- [`database/README.md`](database/README.md)
- [`docker/README.md`](docker/README.md)
- [`artifacts/README.md`](artifacts/README.md)
