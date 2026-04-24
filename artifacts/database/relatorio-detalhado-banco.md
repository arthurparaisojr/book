# Relatorio Detalhado do Banco

## 1. Objetivo

Consolidar a documentacao tecnica do banco do projeto `Book`, descrevendo estrutura,
integridade relacional, objetos de apoio ao relatorio e estrategia de auditoria.

## 2. Banco adotado

- Banco: `SQL Server 2022 Developer`
- Execucao local: `Docker Desktop` no `Windows 11` com integracao WSL
- Base padrao: `BookDb`

## 3. Ordem oficial de execucao

1. `database/schema/001_create_base_tables.sql`
2. `database/schema/002_create_indexes_and_constraints.sql`
3. `database/schema/003_create_audit_tables.sql`
4. `database/views/001_vw_relatorio_livros_por_autor.sql`
5. `database/procedures/001_pr_livro_obter_por_filtros.sql`
6. `database/procedures/002_pr_relatorio_livros_por_autor.sql`
7. `database/triggers/001_trg_livro_audit.sql`
8. `database/seeds/001_seed_initial_data.sql`

## 4. Tabelas principais

### 4.1 `dbo.Livro`

Funcao:

- armazenar os dados centrais do livro.

Campos:

- `Codl INT IDENTITY(1,1)` `PK`
- `Titulo VARCHAR(40)` `NOT NULL`
- `Editora VARCHAR(40)` `NOT NULL`
- `Edicao INT` `NOT NULL`
- `AnoPublicacao CHAR(4)` `NOT NULL`
- `Valor DECIMAL(10,2)` `NOT NULL`
- `DataCriacao DATETIME2(0)` `NOT NULL`
- `DataAtualizacao DATETIME2(0)` `NULL`

Regras:

- `DF_Livro_Valor` com valor padrao `0`
- `DF_Livro_DataCriacao` com `SYSUTCDATETIME()`
- `CK_Livro_AnoPublicacao` para permitir apenas 4 digitos
- `CK_Livro_Valor` para impedir valor negativo
- `IX_Livro_Titulo` para pesquisa por titulo

### 4.2 `dbo.Autor`

Funcao:

- armazenar autores relacionados aos livros.

Campos:

- `CodAu INT IDENTITY(1,1)` `PK`
- `Nome VARCHAR(40)` `NOT NULL`

Regras:

- `IX_Autor_Nome` para pesquisa por nome

### 4.3 `dbo.Assunto`

Funcao:

- armazenar assuntos vinculados aos livros.

Campos:

- `codAs INT IDENTITY(1,1)` `PK`
- `Descricao VARCHAR(20)` `NOT NULL`

Regras:

- `IX_Assunto_Descricao` para pesquisa por descricao

## 5. Tabelas de relacionamento

### 5.1 `dbo.Livro_Autor`

Funcao:

- implementar o relacionamento N:N entre livro e autor.

Chaves:

- `PK_Livro_Autor (Livro_Codl, Autor_CodAu)`
- `FK_Livro_Autor_Livro`
- `FK_Livro_Autor_Autor`

Indices:

- `IX_Livro_Autor_Autor`

Observacao:

- os `FKs` usam `ON DELETE CASCADE` para evitar lixo relacional quando um livro ou
  autor e removido.

### 5.2 `dbo.Livro_Assunto`

Funcao:

- implementar o relacionamento N:N entre livro e assunto.

Chaves:

- `PK_Livro_Assunto (Livro_Codl, Assunto_codAs)`
- `FK_Livro_Assunto_Livro`
- `FK_Livro_Assunto_Assunto`

Indices:

- `IX_Livro_Assunto_Assunto`

Observacao:

- os `FKs` usam `ON DELETE CASCADE`.

## 6. Tabela de auditoria

### 6.1 `dbo.Livro_Audit`

Funcao:

- registrar operacoes `INSERT`, `UPDATE` e `DELETE` sobre `Livro`.

Campos:

- `AuditId BIGINT IDENTITY(1,1)` `PK`
- `Acao VARCHAR(10)` `NOT NULL`
- `Codl INT` `NOT NULL`
- `TituloAnterior VARCHAR(40)` `NULL`
- `TituloNovo VARCHAR(40)` `NULL`
- `ValorAnterior DECIMAL(10,2)` `NULL`
- `ValorNovo DECIMAL(10,2)` `NULL`
- `AlteradoEm DATETIME2(0)` `NOT NULL`

## 7. View de relatorio

### 7.1 `dbo.vw_RelatorioLivrosPorAutor`

Objetivo:

- fornecer a base consolidada do relatorio exigido no projeto.

Caracteristicas:

- junta `Livro`, `Autor`, `Livro_Autor`, `Livro_Assunto` e `Assunto`;
- agrupa o resultado por autor e livro;
- consolida assuntos com `STRING_AGG`;
- expoe `CodAu`, `AutorNome`, `Codl`, `Titulo`, `Editora`, `Edicao`,
  `AnoPublicacao`, `Valor` e `Assuntos`.

Uso esperado:

- relatorio principal agrupado por autor;
- consultas de apoio para frontend ou exportacao.

## 8. Procedures

### 8.1 `dbo.pr_Livro_ObterPorFiltros`

Objetivo:

- consultar livros por filtros opcionais de titulo, autor e assunto.

Entradas:

- `@Titulo`
- `@AutorNome`
- `@AssuntoDescricao`

Saida:

- lista distinta de livros com campos basicos e valor.

### 8.2 `dbo.pr_RelatorioLivrosPorAutor`

Objetivo:

- consultar a view de relatorio com filtro opcional por autor.

Entrada:

- `@AutorNome`

Saida:

- conjunto ordenado por `AutorNome` e `Titulo`.

## 9. Trigger

### 9.1 `dbo.trg_Livro_Audit`

Objetivo:

- auditar alteracoes em `Livro`.

Eventos tratados:

- `INSERT`
- `UPDATE`
- `DELETE`

Estrategia:

- registra acao executada;
- guarda antes e depois de `Titulo` e `Valor`;
- usa `inserted` e `deleted` para diferenciar cada operacao.

## 10. Seeds

O seed inicial cria:

- 3 autores
- 3 assuntos
- 2 livros
- 2 relacoes livro/autor
- 2 relacoes livro/assunto

Observacoes:

- o script usa transacao explicita;
- `SET XACT_ABORT ON` impede carga parcial em caso de erro.

## 11. Evidencias esperadas na apresentacao

- carga bem-sucedida dos scripts
- consulta da `vw_RelatorioLivrosPorAutor`
- execucao das `procedures`
- prova de auditoria em `Livro_Audit`
- comprovacao de constraints e relacionamentos

## 12. Validacao recomendada

Use em conjunto:

- `artifacts/database/checklist-validacao-banco.md`
- `scripts/validate-database.sh`
