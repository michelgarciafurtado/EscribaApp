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

## Modelagem do domínio

- `Projeto` é a raiz do agregado e controla o ciclo de vida de Personagem,
  Quadro, Bloco e PontoChave. Somente o agregado expõe operações públicas de alteração.
- Entidades têm identidade `Guid` gerada pelo domínio, construtores internos
  e propriedades sem setters públicos. Coleções são expostas por visões somente de leitura.
- Genero, ObjetivoNarrativo e LigacaoQuadros são objetos de valor imutáveis.
  Seus records possuem propriedades somente de leitura, sem `init` público.
- As ligações ficam em uma única coleção no agregado. Não existem listas
  paralelas de antecessores e sucessores em Quadro. Consultas derivam das ligações.
- A escaleta é um grafo direcionado sem ciclos; permite ramificações,
  convergências e componentes desconectados enquanto a obra está em planejamento.
- A ordenação topológica produz uma sequência válida, sem escolher um caminho
  narrativo nem enumerar todas as combinações possíveis.

## Resultados de operações

Adotados `Resultado`, `Resultado<T>` e `Erro`, implementados uma única vez em
`EscribaApp.Dominio/Resultados`, sem pacote externo. Falhas previstas retornam
código e mensagem em português. Consultar `Valor` em resultado de falha é uma
violação de contrato e lança `InvalidOperationException`.

Consultas de coleções retornam coleções vazias quando não há correspondências.
`ObterQuadro` retorna `Resultado<Quadro>` para distinguir um identificador inexistente.
Operações do domínio são síncronas, pois não realizam acesso externo.

## Testes

O projeto `testes/EscribaApp.Dominio.Tests`, ao lado de `app`, utiliza NUnit.
NUnit, seu adaptador e Microsoft.NET.Test.Sdk são dependências exclusivas dos
testes. O projeto de domínio continua sem referências externas.
