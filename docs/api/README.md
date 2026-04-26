# API

## Objetivo

Definir o padrao da API REST do projeto `Book` para que ela seja previsivel, bem
documentada e segura para manutencao.

## Padrao de rotas

- Base: `/api/v1`
- Recursos principais:
  - `/api/v1/livros`
  - `/api/v1/autores`
  - `/api/v1/assuntos`
  - `/api/v1/relatorios/livros-por-autor`

## Regras de implementacao

- Controllers finos e servicos de aplicacao responsaveis pela regra de orquestracao.
- DTOs separados para entrada, saida e filtros.
- Validacao de entrada antes da regra de negocio.
- Respostas com status code coerente.
- Autenticacao `JWT Bearer` como padrao oficial.
- Autorizacao por policy, perfil ou claim quando necessario.
- Swagger sempre atualizado.
- Uso de `CancellationToken` e async/await.
- Relatorios podem usar `view` do banco como fonte oficial de leitura.

## Autenticacao

Padrao oficial:

- `JWT Bearer` na API `.NET 8`;
- endpoint de login para emissao do token;
- protecao de endpoints sensiveis com `[Authorize]`;
- segredo JWT fora do codigo-fonte;
- suporte futuro a `refresh token` se o escopo exigir.

## Tratamento de excecoes

Padrao minimo esperado:

- middleware global de excecao;
- retorno no formato `ProblemDetails`;
- mapeamento especifico para:
  - violacao de chave unica;
  - violacao de FK;
  - registro nao encontrado;
  - regra de negocio invalida;
  - falha de validacao.

## Observabilidade

- `Serilog` para logs estruturados;
- `correlation-id` por requisicao;
- `health checks` para API e banco;
- eventos de autenticacao relevantes auditados em log sem expor dados sensiveis;
- logs sem expor senha, connection string ou stack trace ao cliente.

## Documentacao recomendada

- `Swagger UI` para navegacao;
- `OpenAPI` exportado em `artifacts/api/`;
- exemplos de request/response;
- descricao de filtros, paginacao e erros.
