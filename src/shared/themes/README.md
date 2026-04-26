# Themes

Biblioteca oficial de temas CSS do projeto `Book`.

## Objetivo

- centralizar variaveis visuais;
- manter o tema padrao em azuis;
- permitir reuso entre Angular e React;
- evitar divergencia visual entre modulos.

## Abordagem recomendada

O tema deve ser montado de forma inteligente, em camadas:

1. `paleta base`
   - tons de azul, neutros e estados.
2. `tokens semanticos`
   - fundo, superficie, texto, borda, foco, destaque, erro e sucesso.
3. `componentes-base`
   - botoes, cards, inputs, tabelas, badges e cabecalhos.

## Regras de UX

- linguagem visual amigavel e clara;
- contraste suficiente para leitura;
- foco visivel para navegacao por teclado;
- hover e estados ativos suaves;
- evitar azul puro agressivo em toda a tela;
- usar gradientes, superfices e sombras com moderacao.

## Arquivos

- `book-default-blue.css`: tema padrao da solucao.

## Regra

- todo frontend deve partir deste tema como base;
- novos temas devem preservar a identidade amigavel do projeto;
- preferir tokens semanticos a cores soltas em componentes;
- evitar hardcode de cor dentro de telas e componentes;
- alteracoes visuais relevantes devem atualizar a documentacao.
