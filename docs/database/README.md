# Banco de Dados

## Padrao adotado

O projeto usara `SQL Server 2022 Developer` executado localmente com `Docker`, com scripts
versionados no repositorio.

## Banco oficial do projeto

O banco oficial definido para a solucao e:

- `PostgreSQL`
- `SQL Server 2022 Developer`

Justificativa:

- aderencia muito boa ao ecossistema `.NET 8`;
- suporte nativo a objetos como `views`, `procedures` e `triggers`;
- facilidade para demonstracao local via `Docker`;
- comportamento previsivel em cenarios relacionais e transacionais;
- uso gratuito em desenvolvimento e testes.

## Observacao de licenciamento

`SQL Server Developer` atende bem ao objetivo do teste porque e gratuito para
desenvolvimento e testes. Caso o projeto avance para producao com exigencia de banco
livre, a estrategia deve ser revisitada.

## Objetos obrigatorios

- tabelas principais: `Livro`, `Autor`, `Assunto`;
- tabelas de relacionamento: `Livro_Autor`, `Livro_Assunto`;
- campo `Valor` em `Livro`;
- `views` para relatorios;
- `procedures` para consultas e operacoes padronizadas;
- `triggers` para auditoria ou rastreabilidade quando necessario;
- `indices` para leitura e relacionamentos;
- `seeds` para ambiente local e demonstracao.

## Modelagem recomendada

- chaves primarias inteiras e simples nas entidades principais;
- chaves compostas nas tabelas N:N;
- `DECIMAL(10,2)` para o valor monetario;
- `CHAR(4)` ou `VARCHAR(4)` para `AnoPublicacao`;
- `FK` com nome explicito e indices de apoio;
- constraints de validacao para ano e valor.

## Relatorio detalhado do banco

O material esperado para apresentacao deve contemplar:

- relacao de tabelas e seus campos;
- PKs, FKs, indices e constraints;
- descricao da `view` do relatorio;
- descricao das `procedures`;
- descricao das `triggers`;
- estrategia de auditoria;
- ordem de execucao dos scripts.

Os scripts ficam em `database/` e as evidencias em `artifacts/database/`.
