# Arquitetura

A aplicação utiliza DDD e arquitetura hexagonal.

## Domain

Contém agregados, entidades, objetos de valor e regras de negócio.

Não depende de Application, Infrastructure ou Api.
Não utiliza tipos do ASP.NET Core ou do Entity Framework Core.

## Application

Contém casos de uso, DTOs e portas necessárias aos casos de uso.

Depende de Domain.
Não depende de Infrastructure ou Api.

## Infrastructure

Implementa persistência, identidade e integrações externas.

Depende de Application e Domain.

## Api

Recebe requisições HTTP e chama os casos de uso.
Configura a injeção de dependências.

Não implementa regras de negócio.
Pode referenciar Infrastructure para registrar suas implementações.