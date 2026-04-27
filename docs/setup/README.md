# Setup

Guia oficial para criar o projeto `Book` do zero.

## 1. Preparar a maquina

Instale:

- `Git`
- `Docker Desktop`
- `.NET SDK 8`
- `Node.js 20 LTS`
- `Angular CLI`
- `SQL Server Management Studio` ou `Azure Data Studio`

## 2. Observacao para Windows + WSL

Este projeto assume o seguinte modelo:

- `Docker Desktop` instalado no Windows;
- WSL com integracao ativa no `Docker Desktop`;
- comandos executados dentro do Ubuntu/WSL;
- navegador aberto no Windows para acessar `Angular`, `React` e `Swagger`.

Nao e necessario instalar navegador no WSL.

## 3. Regra obrigatoria de Docker

Padrao oficial desta solucao:

- usar sempre o `Docker Desktop` do `Windows 11`;
- usar a integracao WSL do `Docker Desktop`;
- nao instalar `Docker Engine`, `containerd` ou `docker compose plugin` dentro do WSL
  para este projeto.

Se existir instalacao local de Docker dentro do Ubuntu, ela deve ser removida para
evitar conflito com o Docker do Windows.

## 4. Clonar e configurar o repositorio

```bash
git clone <url-do-repositorio> book
cd book
git config commit.template .gitmessage
cp docker/.env.example docker/.env
```

## 5. Subir infraestrutura local

```bash
docker compose -f docker/docker-compose.infrastructure.yml up -d
```

Se a integracao WSL estiver correta, esse comando funcionara no Ubuntu usando o
Docker do Windows.

Se aparecer a mensagem `The command 'docker' could not be found in this WSL 2 distro`,
pare e corrija a integracao no `Docker Desktop` antes de continuar. Nessa situacao,
nao adianta seguir para `apply-database.sh`, porque o banco ainda nao foi iniciado.

## 6. Banco escolhido

Banco oficial do projeto:

- `SQL Server 2022 Developer`

Motivos:

- compatibilidade excelente com `.NET 8`;
- suporte completo para `views`, `procedures`, `triggers` e constraints;
- facilidade de execucao com `Docker`;
- gratuidade em desenvolvimento e testes.

## 7. Criar a solucao backend

```bash
dotnet new sln -n Book
dotnet new webapi -n Book.Api -o src/backend/Book.Api
dotnet new classlib -n Book.Application -o src/backend/Book.Application
dotnet new classlib -n Book.Domain -o src/backend/Book.Domain
dotnet new classlib -n Book.Infrastructure -o src/backend/Book.Infrastructure
dotnet new xunit -n Book.Api.Tests -o tests/backend/Book.Api.Tests
dotnet sln add src/backend/Book.Api/Book.Api.csproj
dotnet sln add src/backend/Book.Application/Book.Application.csproj
dotnet sln add src/backend/Book.Domain/Book.Domain.csproj
dotnet sln add src/backend/Book.Infrastructure/Book.Infrastructure.csproj
dotnet sln add tests/backend/Book.Api.Tests/Book.Api.Tests.csproj
dotnet add src/backend/Book.Api/Book.Api.csproj reference src/backend/Book.Application/Book.Application.csproj
dotnet add src/backend/Book.Application/Book.Application.csproj reference src/backend/Book.Domain/Book.Domain.csproj
dotnet add src/backend/Book.Infrastructure/Book.Infrastructure.csproj reference src/backend/Book.Application/Book.Application.csproj
dotnet add src/backend/Book.Infrastructure/Book.Infrastructure.csproj reference src/backend/Book.Domain/Book.Domain.csproj
dotnet add tests/backend/Book.Api.Tests/Book.Api.Tests.csproj reference src/backend/Book.Api/Book.Api.csproj
```

## 8. Criar o frontend Angular

```bash
npx @angular/cli@latest new src/frontend-angular/book-admin --routing --style css
```

## 9. Criar o frontend React

```bash
npm create vite@latest src/frontend-react/book-insights -- --template react-ts
```

## 10. Ajustar idioma e formatos

- backend com cultura `pt-BR`;
- Angular com locale `pt-BR`;
- React com `Intl.NumberFormat` para moeda;
- datas e mensagens padronizadas em portugues;
- mascara monetaria obrigatoria para `Valor`.

## 11. Configurar autenticacao

Padrao oficial:

- API com `JWT Bearer`;
- segredo JWT via variavel de ambiente;
- endpoint de login;
- `[Authorize]` nos endpoints protegidos;
- possibilidade futura de `refresh token`.

Pacotes recomendados:

- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.AspNetCore.Authorization`

## 12. Criar banco e scripts

Executar os scripts versionados nesta ordem:

1. `database/schema`
2. `database/views`
3. `database/procedures`
4. `database/triggers`
5. `database/seeds`

Exemplo:

```bash
docker exec -it book-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Book@123456' -C \
  -i /workspace/database/schema/001_create_base_tables.sql
```

## 13. Configurar a API

Pacotes recomendados:

- `Swashbuckle.AspNetCore`
- `FluentValidation`
- `Serilog.AspNetCore`
- `Dapper`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.AspNetCore.Authentication.JwtBearer`

Implementacoes obrigatorias:

- versionamento de rotas;
- DTOs;
- validacoes;
- autenticacao `JWT Bearer`;
- middleware global de excecao;
- `ProblemDetails`;
- logs estruturados;
- health checks.

## 14. Estrutura Docker completa

Arquivos previstos:

- `docker/docker-compose.infrastructure.yml`
- `docker/docker-compose.fullstack.yml`
- `docker/Dockerfile.api`
- `docker/Dockerfile.angular`
- `docker/Dockerfile.react`
- `docker/.env.example`

Servicos previstos na stack completa:

- `sqlserver`
- `api`
- `frontend-angular`
- `frontend-react`

Com a stack atual, o fluxo recomendado e:

```bash
./scripts/start-fullstack.sh
```

Para parar tudo sem apagar os dados persistidos:

```bash
./scripts/stop-fullstack.sh
```

Para apagar tudo e recriar o ambiente do zero:

```bash
./scripts/reset-fullstack.sh
```

Alternativa manual:

```bash
docker compose -f docker/docker-compose.fullstack.yml up -d --build
./scripts/apply-database.sh
```

Portas esperadas para acesso pelo navegador do Windows:

- API / Swagger: `http://localhost:8080`
- Angular: `http://localhost:4200`
- React: `http://localhost:4173`

Observacao importante:

- o Angular e o React consomem a API por `/api/v1`;
- no desenvolvimento local, `ng serve` e `vite` fazem proxy para `http://localhost:5268`;
- na stack Docker, o `nginx` dos dois frontends faz proxy para o container `api`.
- `stop-fullstack.sh` preserva o volume do banco;
- `reset-fullstack.sh` remove o volume do banco e executa novamente a subida completa.

## 15. Rodar sem Docker fullstack

Se voce quiser executar os modulos manualmente, sem subir Angular, React e API em
containers, use este fluxo:

1. subir apenas o banco:

```bash
cd /home/arthur/github/book
./scripts/start-infra.sh
./scripts/apply-database.sh
```

2. subir a API localmente:

```bash
cd /home/arthur/github/book/src/backend/Book.Api
dotnet run --launch-profile http
```

3. subir o Angular localmente:

```bash
cd /home/arthur/github/book/src/frontend-angular/book-admin
npm install
npm start
```

4. subir o React localmente:

```bash
cd /home/arthur/github/book/src/frontend-react/book-insights
npm install
npm run dev
```

Esse modo continua dependendo do banco em Docker, que e a forma padronizada de manter
o SQL Server reproduzivel no projeto.

## 16. Consolidar artefatos tecnicos

Para preparar a entrega da etapa `6.B`, execute:

```bash
./scripts/prepare-delivery-artifacts.sh
```

Resultado esperado:

- `OpenAPI` exportado em `artifacts/api/book-api-openapi-v1.json`;
- saida da validacao do banco em `artifacts/database/validacao-banco-output.txt`,
  quando o container `book-sqlserver` estiver disponivel;
- roteiro e checklist de apresentacao centralizados em `artifacts/reports/`.

## 17. Construir relatorio

- criar `view` com dados de livro, autor e assunto;
- agrupar por autor;
- expor endpoint ou exportacao para o frontend;
- guardar modelos gerados em `artifacts/reports/`.

## 18. Testar

- `xUnit` para backend;
- `Jest` para Angular;
- `Vitest` para React;
- `Playwright` para fluxo de ponta a ponta.

## 19. Preparar apresentacao

- atualizar documentos;
- exportar Swagger/OpenAPI;
- separar evidencias do banco;
- anexar prints, relatorios e diagrama em `artifacts/`.

## 20. Implementar relatorio obrigatorio TJ-JUD

- criar ou validar a `view` SQL unindo `Livro`, `Autor` e `Assunto`, com agrupamento
  por autor;
- separar a leitura da `view` em uma camada de consulta e a montagem do PDF em uma
  camada de geracao de documento;
- usar abordagem moderna de `HTML -> PDF`, sem `Crystal Reports` ou `ReportViewer`,
  para manter compatibilidade com `Docker` e `WSL2`;
- aplicar `Bootstrap` no layout do relatorio;
- tratar falhas de banco com mapeamento especifico, sem `try-catch` generico na
  chamada ao banco;
- salvar o PDF final e evidencias em `artifacts/reports/`.

## 21. Limpeza opcional do WSL

Se em algum momento voce tiver instalado Docker dentro do Ubuntu/WSL, remova com:

```bash
sudo apt remove -y docker.io docker-doc docker-compose docker-compose-v2 podman-docker containerd runc docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo apt autoremove -y
sudo rm -rf /var/lib/docker /var/lib/containerd
hash -r
```

Depois valide que o WSL esta usando o Docker Desktop do Windows:

```bash
docker version
docker context ls
docker compose version
```

O esperado e que o comando `docker` continue funcionando no WSL por causa da
integracao com o `Docker Desktop`, nao por causa de uma engine instalada dentro do
Ubuntu.
