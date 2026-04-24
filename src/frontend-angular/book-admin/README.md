# Book Admin

Frontend Angular principal do projeto `Book`.

## Papel no projeto

- autenticar com `JWT`;
- operar o CRUD administrativo;
- consultar `Livro`, `Autor` e `Assunto`;
- servir como base do painel principal do cliente.

## O que ja existe

- tela de login;
- shell administrativo;
- dashboard inicial;
- paginas de `Livro`, `Autor` e `Assunto`;
- tema azul compartilhado;
- mascara monetaria em `pt-BR` no campo `Valor`;
- validacoes com mensagens amigaveis e foco acessivel;
- skip link, estados com `aria-live` e icones compartilhados nas acoes;
- integracao com a API em `http://localhost:5268/api/v1`.

## Como executar

```bash
cd /home/arthur/github/book
./scripts/start-infra.sh
./scripts/apply-database.sh
cd src/backend/Book.Api
dotnet run --launch-profile http
```

Em outro terminal:

```bash
cd /home/arthur/github/book/src/frontend-angular/book-admin
npm install
npm start
```

Abra no navegador do Windows:

- `http://localhost:4200`

## Credenciais locais

- `book-admin` / `Book@123`
- `book-reader` / `Book@123`

## Validacao recomendada

1. entrar com `book-admin`;
2. abrir dashboard;
3. listar livros, autores e assuntos;
4. criar ou editar um livro e conferir a mascara de `Valor` no formato `pt-BR`;
5. validar mensagens de erro ao deixar campos obrigatorios em branco;
6. testar o perfil `book-reader` e confirmar `403` em escrita.
