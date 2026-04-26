# Checklist de Validacao do Banco

## Objetivo

Guiar a validacao tecnica da etapa `2.C` antes de seguir para o backend.

## 1. Infraestrutura

- `Docker Desktop` do `Windows 11` ativo
- integracao WSL habilitada
- container `book-sqlserver` em estado `healthy`

Comandos:

```bash
docker compose -f docker/docker-compose.infrastructure.yml ps
docker inspect --format='{{json .State.Health}}' book-sqlserver
```

## 2. Aplicacao dos scripts

- `./scripts/start-infra.sh` executado com sucesso
- `./scripts/apply-database.sh` executado sem erros
- sem truncamento de dados
- sem violacao de `FK`

## 3. Estrutura

Confirmar existencia de:

- `Livro`
- `Autor`
- `Assunto`
- `Livro_Autor`
- `Livro_Assunto`
- `Livro_Audit`

## 4. Integridade

Validar:

- `PK` nas tabelas principais
- `PK` composta nas tabelas N:N
- `FKs` corretas
- `CHECK` de ano e valor
- indices criados

## 5. Dados iniciais

Esperado:

- 3 autores
- 3 assuntos
- 2 livros
- 2 registros em `Livro_Autor`
- 2 registros em `Livro_Assunto`

## 6. View

Validar:

- `dbo.vw_RelatorioLivrosPorAutor` criada
- retorno de linhas com autor, livro, valor e assuntos

## 7. Procedures

Validar:

- `dbo.pr_Livro_ObterPorFiltros`
- `dbo.pr_RelatorioLivrosPorAutor`

## 8. Trigger

Validar:

- `dbo.trg_Livro_Audit` criada
- auditoria gerada em `Livro_Audit` para operacoes em `Livro`

## 9. Script de smoke test

Executar:

```bash
./scripts/validate-database.sh
```

O script deve validar:

- contagem de dados seed
- retorno da view
- execucao das procedures
- disparo da trigger em transacao controlada

## 10. Critrio para avancar

A etapa `2.C` pode ser considerada pronta quando:

- os scripts sobem do zero sem erro;
- o smoke test do banco passa;
- o relatorio detalhado do banco esta atualizado;
- as evidencias estao prontas para apresentacao.
