# Book Insights

Modulo complementar em React do projeto `Book`.

## Papel no projeto

- leitura analitica;
- visao executiva do catalogo;
- apoio de apresentacao;
- experiencia complementar ao Angular administrativo.

## O que ja existe

- dashboard com dados reais da API;
- resumo de saude do backend;
- leitura de livros, autores e assuntos;
- busca local sobre o catalogo carregado;
- uso do tema azul compartilhado;
- reaproveitamento de SVGs da biblioteca do projeto.

## Como executar

1. subir banco e backend:

```bash
cd /home/arthur/github/book
./scripts/start-infra.sh
./scripts/apply-database.sh
cd src/backend/Book.Api
dotnet run --launch-profile http
```

2. em outro terminal, subir o React:

```bash
cd /home/arthur/github/book/src/frontend-react/book-insights
npm install
npm run dev
```

3. abrir no navegador do Windows:

- `http://localhost:5173`

## Observacao

Este modulo e majoritariamente de leitura. A operacao administrativa principal continua no Angular em `http://localhost:4200`.
