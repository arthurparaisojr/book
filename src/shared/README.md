# Shared

Pasta reservada para itens compartilhados entre modulos, como:

- contratos de API;
- design tokens em CSS;
- biblioteca de temas;
- biblioteca de icones SVG;
- enums e constantes;
- documentacao tecnica complementar.

## Estrutura oficial

- `themes/`: temas CSS reutilizaveis, com o padrao visual azul do projeto.
- `icons/svg/`: icones SVG compartilhados entre Angular e React.

## Regra de uso

- o tema padrao da solucao deve sair de `themes/`;
- se um novo SVG for necessario, o nome do arquivo deve ser combinado antes;
- o local oficial de gravacao de SVG e `src/shared/icons/svg/`.
