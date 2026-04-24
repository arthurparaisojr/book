# Backend

Solucao `.NET 8` do projeto `Book`.

## Estrutura recomendada

- `Book.Api`
- `Book.Application`
- `Book.Domain`
- `Book.Infrastructure`

## Responsabilidades

- expor API REST;
- centralizar regras de negocio;
- tratar excecoes;
- documentar Swagger;
- integrar com banco e logs.

## Status atual

- `3.A` concluida com solution, camadas e teste inicial.
- `3.B` em andamento com CRUDs de `Livro`, `Autor` e `Assunto` implementados.
- `3.C` em andamento com autenticacao `JWT`, policy de escrita, health checks e logging de requisicoes.

Endpoints atuais da API:

- `POST /api/v1/auth/login`
- `GET /api/v1/health`
- `GET /api/v1/health/live`
- `GET /api/v1/health/ready`
- `GET /api/v1/assuntos`
- `GET /api/v1/assuntos/{codAs}`
- `POST /api/v1/assuntos`
- `PUT /api/v1/assuntos/{codAs}`
- `DELETE /api/v1/assuntos/{codAs}`
- `GET /api/v1/autores`
- `GET /api/v1/autores/{codAu}`
- `POST /api/v1/autores`
- `PUT /api/v1/autores/{codAu}`
- `DELETE /api/v1/autores/{codAu}`
- `GET /api/v1/livros`
- `GET /api/v1/livros/{codl}`
- `POST /api/v1/livros`
- `PUT /api/v1/livros/{codl}`
- `DELETE /api/v1/livros/{codl}`

## Uso com Visual Studio 2022

Forma recomendada:

1. abrir a solution `Book.sln` na raiz do repositorio;
2. definir `Book.Api` como `Startup Project`;
3. usar o perfil `https` no Visual Studio, se quiser Swagger com HTTPS;
4. executar com `F5` ou `Ctrl+F5`.

Configuracao atual:

- Swagger abre pelo `launchSettings.json`;
- a connection string local de desenvolvimento esta em
  `src/backend/Book.Api/appsettings.Development.json`;
- o SQL Server deve estar rodando no `Docker Desktop` do Windows, exposto em
  `localhost:1433`.
- usuarios locais de desenvolvimento ficam em
  `src/backend/Book.Api/appsettings.Development.json`.

## Teste rapido do endpoint de saude

Se executar pelo terminal com:

```bash
cd src/backend/Book.Api
dotnet run
```

o comportamento padrao mais simples e testar por:

- `http://localhost:5268/api/v1/health`

Se executar explicitamente o perfil HTTPS:

```bash
cd src/backend/Book.Api
dotnet run --launch-profile https
```

entao voce podera testar por:

- `https://localhost:7082/api/v1/health`
- `https://localhost:7082/swagger`

## Como testar a etapa 3.C

### 1. Subir infraestrutura no Docker Desktop do Windows

Na raiz do repositorio:

```bash
./scripts/start-infra.sh
./scripts/apply-database.sh
./scripts/validate-database.sh
```

### 2. Validar compilacao e testes automatizados

Na raiz do repositorio:

```bash
dotnet build Book.sln
dotnet test Book.sln
```

### 3. Subir a API

Para o fluxo mais simples em `http`:

```bash
cd src/backend/Book.Api
dotnet run --launch-profile http
```

### 4. Testar manualmente

Opcoes recomendadas:

- usar `src/backend/Book.Api/Book.Api.http`;
- usar o Swagger em `http://localhost:5268/swagger`;
- ou chamar a API por `curl`.

Credenciais locais de desenvolvimento:

- usuario `book-admin` com senha `Book@123`
- usuario `book-reader` com senha `Book@123`

Exemplo rapido de login:

```bash
curl -X POST http://localhost:5268/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"book-admin","password":"Book@123"}'
```

Resultados esperados desta etapa:

- `POST /api/v1/auth/login` retorna `200 OK` com token JWT para credenciais validas;
- `POST /api/v1/auth/login` retorna `401 Unauthorized` para credenciais invalidas;
- `GET /api/v1/health` retorna `200 OK`;
- `GET /api/v1/health/live` retorna o status da API;
- `GET /api/v1/health/ready` valida a conexao com o banco;
- `GET /api/v1/assuntos` retorna os assuntos seedados;
- `POST /api/v1/assuntos` sem token retorna `401 Unauthorized`;
- `POST /api/v1/assuntos` com token de `Admin` cria um assunto e retorna `201 Created`;
- `PUT /api/v1/assuntos/{codAs}` com token de `Admin` atualiza e retorna `200 OK`;
- `DELETE /api/v1/assuntos/{codAs}` com token de `Admin` retorna `204 No Content`;
- `GET /api/v1/autores` retorna os autores seedados;
- `POST /api/v1/autores` com token de `Admin` cria um autor e retorna `201 Created`;
- `PUT /api/v1/autores/{codAu}` com token de `Admin` atualiza e retorna `200 OK`;
- `DELETE /api/v1/autores/{codAu}` com token de `Admin` retorna `204 No Content`;
- `GET /api/v1/livros` retorna os livros seedados;
- `POST /api/v1/livros` com token de `Admin` cria um livro e retorna `201 Created`;
- `PUT /api/v1/livros/{codl}` com token de `Admin` atualiza e retorna `200 OK`;
- `DELETE /api/v1/livros/{codl}` com token de `Admin` retorna `204 No Content`;
- entradas invalidas retornam `400 Bad Request` com `ProblemDetails`;
- ids inexistentes retornam `404 Not Found`;
- erros e respostas de falha retornam `traceId` para facilitar diagnostico.

## Arquivos importantes

- `src/backend/Book.Api/Properties/launchSettings.json`
- `src/backend/Book.Api/appsettings.json`
- `src/backend/Book.Api/appsettings.Development.json`
- `src/backend/Book.Api/Book.Api.http`
