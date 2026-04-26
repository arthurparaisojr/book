# Processo

## Padrao de commits

Use sempre:

```text
tipo(etapa): descricao curta
```

Exemplos:

- `docs(1.A): documenta arquitetura do projeto`
- `feat(3.B): implementa CRUD de livros`
- `fix(2.B): corrige FK da tabela Livro_Assunto`

## Branches

Padrao recomendado:

- `main`: ramo estavel
- `feature/<etapa>-<tema>`: novas entregas
- `fix/<etapa>-<tema>`: correcoes
- `docs/<etapa>-<tema>`: documentacao

## Merge e rebase

Use `rebase` para atualizar sua branch local antes de abrir PR ou integrar:

```bash
git fetch origin
git rebase origin/main
```

Use `merge --no-ff` para integrar trabalho validado sem perder o contexto da feature:

```bash
git checkout main
git merge --no-ff feature/3-b-api-livros
```

## Regras importantes

- nunca usar `rebase` em branch compartilhada sem alinhamento;
- nunca apagar historico para esconder erro;
- toda alteracao relevante exige documentacao correspondente;
- toda entrega deve informar etapa e impacto no commit;
- README, `.cursorroles` e regras do Cursor devem ser atualizados quando o processo
  mudar.
