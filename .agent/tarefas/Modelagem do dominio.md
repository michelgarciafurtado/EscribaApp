# Tarefa: Criar o projeto

## Objetivo

- Criar a estrutura de pastas de acordo com padrão hexagonal, importar as bibliotecas de IdentityDb e PostgreSql no projeto de infra.
- O padrão hexagonal deverá estar dentro de uma pasta app.
  - o padrão de nome dos projetos deverá ser EscribaApp.Nome_do_projeto.
  - dentro de app deverão ter as pastas src e adapters.
  - os projetos Dominio e Aplicacao deverão estar dentro de src e infra e web dentro de adapters.
  - os projetos Dominio, Aplicacao, Infra deverão ser do tipo biblioteca de classes.
  - o projeto web devera ser do tipo razor page.
  - Dentro de Dominio deverão estar as pastas Agregados, Entidades e ObjetosValor

## Comportamento esperado

- Que o projeto esteja com a slnx e os projetos de bibliotecas de classe e WebApi nas Pastas e projetos criados no padrão Hexagonal.

## Critérios de aceite

- Não criação de dependencias para o Domain.
- Projetos respeitando as regras de referência propostas pela metodologia DDD.
- Uso da lingua portuguesa.
