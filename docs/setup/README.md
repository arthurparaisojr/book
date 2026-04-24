# Setup

Guia oficial para criar o projeto `Book` do zero.

## 1. Preparar a maquina

Instale:

- `Git`
- `Docker Desktop`
- `.NET SDK 8`
- `Node.js 20 LTS`
- `Angular CLI`
- `SQL Server Management Studio` ou `Azure Data Studio`

## 2. Clonar e configurar o repositorio

```bash
git clone <url-do-repositorio> book
cd book
git config commit.template .gitmessage
cp docker/.env.example docker/.env
```

## 3. Subir infraestrutura local

```bash
docker compose -f docker/docker-compose.infrastructure.yml up -d
```

## 4. Banco escolhido

Banco oficial do projeto:

- `SQL Server 2022 Developer`

Motivos:

- compatibilidade excelente com `.NET 8`;
- suporte completo para `views`, `procedures`, `triggers` e constraints;
- facilidade de execucao com `Docker`;
- gratuidade em desenvolvimento e testes.

## 5. Criar a solucao backend

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
dotnet add src/backend/Book.Api/Book.Api.csproj reference src/backend/Book.Application/Book.Application.csproj
dotnet add src/backend/Book.Application/Book.Application.csproj reference src/backend/Book.Domain/Book.Domain.csproj
dotnet add src/backend/Book.Infrastructure/Book.Infrastructure.csproj reference src/backend/Book.Application/Book.Application.csproj
dotnet add src/backend/Book.Infrastructure/Book.Infrastructure.csproj reference src/backend/Book.Domain/Book.Domain.csproj
dotnet add tests/backend/Book.Api.Tests/Book.Api.Tests.csproj reference src/backend/Book.Api/Book.Api.csproj
```

## 6. Criar o frontend Angular

```bash
npx @angular/cli@latest new src/frontend-angular/book-admin --routing --style css
```

## 7. Criar o frontend React

```bash
npm create vite@latest src/frontend-react/book-insights -- --template react-ts
```

## 8. Ajustar idioma e formatos

- backend com cultura `pt-BR`;
- Angular com locale `pt-BR`;
- React com `Intl.NumberFormat` para moeda;
- datas e mensagens padronizadas em portugues;
- mascara monetaria obrigatoria para `Valor`.

## 9. Configurar autenticacao

Padrao oficial:

- API com `JWT Bearer`;
- segredo JWT via variavel de ambiente;
- endpoint de login;
- `[Authorize]` nos endpoints protegidos;
- possibilidade futura de `refresh token`.

Pacotes recomendados:

- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.AspNetCore.Authorization`

## 10. Criar banco e scripts

Executar os scripts versionados nesta ordem:

1. `database/schema`
2. `database/views`
3. `database/procedures`
4. `database/triggers`
5. `database/seeds`

Exemplo:

```bash
docker exec -it book-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Book@123456' -C \
  -i /workspace/database/schema/001_create_base_tables.sql
```

## 11. Configurar a API

Pacotes recomendados:

- `Swashbuckle.AspNetCore`
- `FluentValidation`
- `Serilog.AspNetCore`
- `Dapper`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.AspNetCore.Authentication.JwtBearer`

Implementacoes obrigatorias:

- versionamento de rotas;
- DTOs;
- validacoes;
- autenticacao `JWT Bearer`;
- middleware global de excecao;
- `ProblemDetails`;
- logs estruturados;
- health checks.

## 12. Estrutura Docker completa

Arquivos previstos:

- `docker/docker-compose.infrastructure.yml`
- `docker/docker-compose.fullstack.template.yml`
- `docker/.env.example`

Servicos previstos na stack completa:

- `sqlserver`
- `sqlserver`
- `api`
- `frontend-angular`
- `frontend-react`

Quando os projetos reais estiverem criados, a stack completa podera ser ligada com:

```bash
docker compose -f docker/docker-compose.fullstack.template.yml up --build
```

## 13. Construir relatorio

- criar `view` com dados de livro, autor e assunto;
- agrupar por autor;
- expor endpoint ou exportacao para o frontend;
- guardar modelos gerados em `artifacts/reports/`.

## 14. Testar

- `xUnit` para backend;
- `Jest` para Angular;
- `Vitest` para React;
- `Playwright` para fluxo de ponta a ponta.

## 15. Preparar apresentacao

- atualizar documentos;
- exportar Swagger/OpenAPI;
- separar evidencias do banco;
- anexar prints, relatorios e diagrama em `artifacts/`.
