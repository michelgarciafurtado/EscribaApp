# Arquitetura

A aplicação utiliza DDD e arquitetura hexagonal.

## Dominio

Contém agregados, entidades, objetos de valor e regras de negócio.

Não depende de Aplicacao, Infraestrutura ou Api.
Não utiliza tipos do ASP.NET Core ou do Entity Framework Core.

## Aplicacao

Contém casos de uso, DTOs e portas necessárias aos casos de uso.

Depende de Dominio.
Não depende de Infraestrutura ou Api.

## Infraestrutura

Implementa persistência, identidade e integrações externas.

Depende de Application e Dominio.

## web

Recebe requisições HTTP e chama os casos de uso.
Configura a injeção de dependências.

Não implementa regras de negócio.
Pode referenciar Infraestrutura para registrar suas implementações.