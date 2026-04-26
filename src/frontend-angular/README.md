# Frontend Angular

Esta area sera usada pelo frontend principal do sistema.

## Papel no projeto

- CRUD de `Livro`, `Autor` e `Assunto`;
- navegacao principal;
- formularios com validacao;
- mascara monetaria para `Valor`;
- consumo da API `.NET`.

## Justificativa de uso

O `Angular` foi escolhido como frontend principal porque o projeto exige um painel
operacional com CRUD, formularios, validacoes, autenticacao e navegacao
administrativa bem estruturada.

## Diretrizes

- idioma `pt-BR`;
- CSS limpo, responsivo e amigavel;
- uso do tema padrao azul compartilhado em `src/shared/themes/`;
- uso de tokens semanticos em vez de cores hardcoded;
- foco visivel, estados suaves e formularios claros;
- uso da biblioteca de icones SVG em `src/shared/icons/svg/`;
- componentes reutilizaveis;
- tratamento visual de erros de validacao.

## Status atual da etapa 4.C

O app `book-admin` agora possui:

- login com `JWT`;
- login manual por usuario e senha, com os mesmos usuarios locais do modulo React;
- shell administrativo;
- dashboard inicial;
- telas para `Livro`, `Autor` e `Assunto`;
- consumo da API `.NET 8` por `/api/v1`, com proxy local para `http://localhost:5268`;
- tema azul compartilhado importado de `src/shared/themes/book-default-blue.css`;
- mascara monetaria em `pt-BR` no cadastro de `Livro`;
- validacoes com mensagens claras e estados acessiveis;
- uso dos SVGs compartilhados nas acoes de interface.

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

## Como executar via Docker

```bash
cd /home/arthur/github/book
./scripts/start-fullstack.sh
```

Acesso:

- `http://localhost:4200`

## Credenciais locais

- `book-admin` / `Book@123`
- `book-reader` / `Book@123`

As mesmas credenciais tambem funcionam no modulo React em `http://localhost:5173`.

## Validacao recomendada da 4.C

1. abrir `http://localhost:4200`;
2. usar o link "Pular para o conteudo principal" com o teclado;
3. testar o login com erro de campo vazio;
4. criar ou editar um livro e confirmar a mascara de `Valor` em `pt-BR`;
5. validar as mensagens de erro dos formularios de `Autor` e `Assunto`.

## Observacao importante

O backend esta configurado para aceitar `CORS` local de:

- `http://localhost:4200`
- `http://localhost:5173`
