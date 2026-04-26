# Scripts

Esta pasta deve concentrar scripts operacionais reutilizaveis, por exemplo:

- inicializacao local;
- carga de dados;
- exportacao de artefatos;
- empacotamento para apresentacao.

## Scripts incluidos

- `start-infra.sh`: copia `docker/.env.example` se necessario, sobe o banco local
  e aguarda o container ficar `healthy` antes de concluir.
- `apply-database.sh`: executa todos os scripts SQL versionados na ordem oficial,
  aguardando o SQL Server ficar estavel e repetindo tentativas transitórias.
- `validate-database.sh`: executa um smoke test do banco para a etapa `2.C`,
  tambem aguardando o container ficar `healthy`.

## Comportamento esperado

- todos os scripts assumem `Docker Desktop` no `Windows 11` com integracao WSL;
- se o comando `docker` nao estiver disponivel no WSL, os scripts devem falhar
  rapidamente com orientacao de correcao;
- `apply-database.sh` e `validate-database.sh` dependem do container criado por
  `start-infra.sh`.

Sempre que um script nascer, documente sua finalidade e forma de uso.
