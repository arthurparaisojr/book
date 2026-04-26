# Seguranca

## Autenticacao oficial

O projeto `Book` adotara `JWT Bearer` como padrao oficial de autenticacao da API
`.NET 8`.

## Motivos da escolha

- integracao nativa e madura com `ASP.NET Core`;
- boa compatibilidade com `Angular` e `React`;
- simplicidade para APIs REST;
- documentacao clara no `Swagger`;
- facilidade de evolucao futura para `refresh token`.

## Diretrizes

- endpoints publicos devem ser minimos e justificados;
- endpoints de manutencao devem exigir token valido;
- usar `[Authorize]` nos recursos protegidos;
- concentrar configuracao de autenticacao e autorizacao na API;
- nao expor segredo JWT em codigo-fonte;
- armazenar segredo em variaveis de ambiente ou secret manager;
- diferenciar erro `401` de `403`.

## Fluxo recomendado

1. usuario autentica;
2. API valida credenciais;
3. API emite `JWT`;
4. frontend envia `Authorization: Bearer <token>`;
5. API aplica autenticacao e autorizacao por policy ou perfil.

## Evolucao opcional

Se o projeto crescer, podera ser acrescentado:

- `refresh token`;
- rotacao de segredos;
- auditoria de login;
- controle por perfis e claims.
