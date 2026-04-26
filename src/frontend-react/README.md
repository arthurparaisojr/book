# Frontend React

Esta area sera usada pelo modulo complementar em `React`.

## Papel no projeto

- dashboard;
- relatorios;
- visualizacao analitica;
- laboratorio de componentes ou experiencia adicional.

## Justificativa de uso

O `React` foi mantido como modulo complementar porque atende muito bem o contexto de
dashboard, leitura analitica, apresentacao executiva e experimentacao de interfaces
mais orientadas a visualizacao do que a operacao transacional.

## Observacao

O React complementa a solucao e nao substitui o frontend principal em Angular.
No menu, os rotulos do React devem deixar claro que se tratam de secoes de
insight e leitura, e nao de operacao.

## Diretrizes visuais

- usar o tema padrao azul compartilhado em `src/shared/themes/`;
- usar tokens semanticos e estilos compartilhados em CSS;
- manter estados visuais suaves e leitura facil;
- reutilizar icones SVG de `src/shared/icons/svg/`;
- manter linguagem visual amigavel e consistente com o Angular.

## Status atual da etapa 4.C

O app `book-insights` agora possui:

- login com usuario e senha consumindo o mesmo endpoint de autenticacao do Angular;
- dashboard de leitura;
- resumo da saude da API;
- indicadores de livros, autores e assuntos;
- leitura de editoras e distribuicao por ano;
- busca local sobre o catalogo carregado;
- uso do tema azul compartilhado;
- skip link e anuncios de carregamento;
- favicon compartilhado do projeto;
- reaproveitamento de SVGs da biblioteca do projeto.

## Evolucao na etapa 5.A

O `book-insights` passou a exibir o relatorio de livros por autor consumindo o
endpoint `/api/v1/relatorios/livros-por-autor`, baseado na `view` oficial do banco.

## Evolucao na etapa 6.A

O `book-insights` agora tambem funciona na stack Docker completa consumindo a API por
`/api/v1`, com proxy no `vite` para o desenvolvimento local e proxy no `nginx` para a
execucao containerizada.

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

- o modulo esta alinhado para rodar sem aviso de engine em `Node.js 20 LTS`,
  incluindo ambientes `20.18.x`.

3. abrir no navegador do Windows:

- `http://localhost:5173`

## Credenciais locais

- `book-admin` / `Book@123`
- `book-reader` / `Book@123`

As mesmas credenciais tambem funcionam no Angular em `http://localhost:4200`.

## Como executar via Docker

```bash
cd /home/arthur/github/book
./scripts/start-fullstack.sh
```

Acesso:

- `http://localhost:4173`

## Validacao recomendada da 4.C

1. abrir `http://localhost:5173`;
2. navegar por teclado e usar o skip link;
3. testar a busca local do catalogo;
4. confirmar o favicon e a consistencia visual com o Angular.

## Validacao recomendada da 5.A

1. abrir `http://localhost:5173`;
2. localizar a secao `Relatorio por Autor`;
3. validar a tabela com autor, titulo, editora, ano, valor e assuntos;
4. testar o filtro textual do relatorio.
