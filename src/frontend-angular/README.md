# Frontend Angular

Esta area sera usada pelo frontend principal do sistema.

## Papel no projeto

- CRUD de `Livro`, `Autor` e `Assunto`;
- navegacao principal;
- formularios com validacao;
- mascara monetaria para `Valor`;
- consumo da API `.NET`.

## Diretrizes

- idioma `pt-BR`;
- CSS limpo, responsivo e amigavel;
- uso do tema padrao azul compartilhado em `src/shared/themes/`;
- uso de tokens semanticos em vez de cores hardcoded;
- foco visivel, estados suaves e formularios claros;
- uso da biblioteca de icones SVG em `src/shared/icons/svg/`;
- componentes reutilizaveis;
- tratamento visual de erros de validacao.

## Status atual da etapa 4.A

O app `book-admin` agora possui:

- login com `JWT`;
- shell administrativo;
- dashboard inicial;
- telas para `Livro`, `Autor` e `Assunto`;
- consumo da API `.NET 8` em `http://localhost:5268/api/v1`;
- tema azul compartilhado importado de `src/shared/themes/book-default-blue.css`.

## Como executar

1. subir a infraestrutura e a API:

```bash
cd /home/arthur/github/book
./scripts/start-infra.sh
./scripts/apply-database.sh
cd src/backend/Book.Api
dotnet run --launch-profile http
```

2. em outro terminal, subir o Angular:

```bash
cd /home/arthur/github/book/src/frontend-angular/book-admin
npm install
npm start
```

3. acessar no navegador do Windows:

- `http://localhost:4200`

## Credenciais locais

- `book-admin` / `Book@123`
- `book-reader` / `Book@123`

## Observacao importante

O backend esta configurado para aceitar `CORS` local de:

- `http://localhost:4200`
- `http://localhost:5173`
