# Backend

Espaco reservado para a solucao `.NET 8`.

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

## Arquivos importantes

- `src/backend/Book.Api/Properties/launchSettings.json`
- `src/backend/Book.Api/appsettings.json`
- `src/backend/Book.Api/appsettings.Development.json`
- `src/backend/Book.Api/Book.Api.http`
