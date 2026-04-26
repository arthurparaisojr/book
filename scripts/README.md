# Scripts

Esta pasta deve concentrar scripts operacionais reutilizaveis, por exemplo:

- inicializacao local;
- carga de dados;
- exportacao de artefatos;
- empacotamento para apresentacao.

## Scripts incluidos

- `start-infra.sh`: copia `docker/.env.example` se necessario, sobe o banco local
  e aguarda o container ficar `healthy` antes de concluir.
- `start-fullstack.sh`: inicia a stack completa do projeto, com banco, API,
  frontend Angular e frontend React, aplica os scripts do banco e informa as URLs.
- `stop-fullstack.sh`: para toda a stack completa e remove containers e rede, mas
  preserva o volume do banco para retomada rapida depois.
- `reset-fullstack.sh`: apaga a stack completa, remove tambem o volume do banco e
  sobe tudo novamente do zero com o schema e os seeds oficiais.
- `apply-database.sh`: executa todos os scripts SQL versionados na ordem oficial,
  aguardando o SQL Server ficar estavel e repetindo tentativas transitórias.
- `validate-database.sh`: executa um smoke test do banco para a etapa `2.C`,
  tambem aguardando o container ficar `healthy`.

## Quando usar cada script

- `start-infra.sh`: quando voce quer subir apenas o SQL Server para trabalhar com
  backend, banco, scripts SQL ou validacoes isoladas.
- `start-fullstack.sh`: quando voce quer subir a aplicacao inteira para uso normal
  ou demonstracao.
- `stop-fullstack.sh`: quando voce terminou de usar a stack e quer parar tudo sem
  perder os dados atuais do banco no volume Docker.
- `reset-fullstack.sh`: quando voce quer recomecar o ambiente do zero, apagando os
  dados persistidos do SQL Server e recriando toda a stack com os seeds oficiais.

## Comportamento esperado

- todos os scripts assumem `Docker Desktop` no `Windows 11` com integracao WSL;
- se o comando `docker` nao estiver disponivel no WSL, os scripts devem falhar
  rapidamente com orientacao de correcao;
- `apply-database.sh` e `validate-database.sh` dependem do container criado por
  `start-infra.sh` ou `start-fullstack.sh`.

Sempre que um script nascer, documente sua finalidade e forma de uso.
