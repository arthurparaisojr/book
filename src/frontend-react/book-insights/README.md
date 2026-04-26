# Book Insights

Modulo complementar em React do projeto `Book`.

## Papel no projeto

- leitura analitica;
- visao executiva do catalogo;
- apoio de apresentacao;
- experiencia complementar ao Angular administrativo.

## O que ja existe

- dashboard com dados reais da API;
- login com usuario e senha usando o mesmo backend JWT do Angular;
- resumo de saude do backend;
- leitura de livros, autores e assuntos;
- relatorio de livros por autor baseado na `view` do banco;
- busca local sobre o catalogo carregado;
- uso do tema azul compartilhado;
- skip link, busca com ajuda textual e anuncios de carregamento;
- favicon compartilhado do projeto;
- reaproveitamento de SVGs da biblioteca do projeto;
- integracao com a API por `/api/v1`, com proxy local no `vite` e proxy containerizado
  no `nginx`.

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

Observacao de toolchain:

- o projeto esta configurado para funcionar bem em `Node.js 20 LTS`, inclusive
  quando o ambiente local ainda estiver em `20.18.x`.

3. abrir no navegador do Windows:

- `http://localhost:5173`

## Credenciais locais

- `book-admin` / `Book@123`
- `book-reader` / `Book@123`

## Como executar via Docker

```bash
cd /home/arthur/github/book
./scripts/start-fullstack.sh
```

Abra no navegador do Windows:

- `http://localhost:4173`

## Observacao

Este modulo e majoritariamente de leitura. A operacao administrativa principal continua no Angular em `http://localhost:4200`.

## Menu do React

Os itens do menu do React representam secoes analiticas, nao telas de CRUD:

- `Visao Geral`
- `Insights de Livros`
- `Relatorio por Autor`
- `Insights de Autores`
- `Insights de Assuntos`

## Validacao recomendada

1. abrir `http://localhost:5173`;
2. usar `Tab` e confirmar o link "Pular para o conteudo principal";
3. validar os cards, o painel de saude e os graficos;
4. abrir a secao `Relatorio por Autor` e validar a tabela baseada na `view`;
5. usar a busca local do relatorio e do catalogo;
6. verificar se o favicon do navegador ja aparece com o icone do projeto.
