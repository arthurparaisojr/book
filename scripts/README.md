# Scripts

Esta pasta deve concentrar scripts operacionais reutilizaveis, por exemplo:

- inicializacao local;
- carga de dados;
- exportacao de artefatos;
- empacotamento para apresentacao.

## Scripts incluidos

- `start-infra.sh`: copia `docker/.env.example` se necessario e sobe o banco local.
- `apply-database.sh`: executa todos os scripts SQL versionados na ordem oficial.

Sempre que um script nascer, documente sua finalidade e forma de uso.
