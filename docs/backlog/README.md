# Backlog

O backlog do projeto `Book` deve seguir a estrutura oficial por etapas numeradas e
subetapas por letra.

## 1. Fundacao

- `1.A` Definir escopo, stack, convencoes, documentacao inicial e estrutura do repo.
- `1.B` Configurar Git, template de commit, padrao de branch e regras de trabalho.

## 2. Banco de dados

- `2.A` Traduzir o modelo de dados em schema relacional, incluindo `Valor` em `Livro`.
- `2.B` Criar scripts de tabelas, constraints, indices, views, procedures, triggers e
  seeds.
- `2.C` Consolidar relatorio detalhado do banco e evidencias tecnicas.

## 3. Backend

- `3.A` Criar a solucao `.NET` e separar camadas `API`, `Application`, `Domain` e
  `Infrastructure`.
- `3.B` Implementar endpoints REST, validacoes, filtros e tratamento de excecoes.
- `3.C` Adicionar logging, health checks, autenticacao `JWT Bearer` e documentacao OpenAPI.

## 4. Frontend

- `4.A` Implementar o frontend Angular para CRUD, navegacao e formularios.
- `4.B` Implementar o frontend React para relatorios, dashboard ou modulo
  complementar.
- `4.C` Aplicar CSS, acessibilidade, responsividade, idioma e mascara monetaria,
  incluindo skip links, mensagens amigaveis e tema azul compartilhado.

## 5. Qualidade e relatorios

- `5.A` Implementar relatorio consumindo `view` do banco.
- `5.B` Criar testes unitarios, integracao e ponta a ponta.

## 6. Entrega

- `6.A` Orquestrar infraestrutura com Docker e preparar stack completa com banco, API e frontends.
- `6.B` Consolidar scripts, artefatos e roteiro de apresentacao.
- `6.C` Revisar backlog entregue, atualizar documentos e validar demonstracao final.

## Evolucoes futuras

Itens abaixo ficam registrados como proxima fase, sem alterar o cronograma oficial
de `1.A` a `6.C`:

- autenticar uma vez e reaproveitar a sessao entre `Angular` e `React`;
- integrar mais a experiencia entre os dois frontends, sem remover o modulo `React`;
- manter `Angular` como modulo operacional e `React` como modulo complementar de
  leitura e analise.

## Criterios transversais

- toda alteracao precisa citar a etapa no commit;
- toda etapa finalizada deve atualizar artefatos e documentacao;
- excecoes, mascaras e idioma `pt-BR` fazem parte do backlog tecnico, nao sao opcionais.
