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
- skip link, busca com ajuda textual e anuncios de carregamento;
- favicon compartilhado do projeto;
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

## Validacao recomendada

1. abrir `http://localhost:5173`;
2. usar `Tab` e confirmar o link "Pular para o conteudo principal";
3. validar os cards, o painel de saude e os graficos;
4. usar a busca local e confirmar o filtro sobre os livros mais recentes;
5. verificar se o favicon do navegador ja aparece com o icone do projeto.
