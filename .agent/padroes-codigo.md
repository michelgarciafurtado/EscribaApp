# 📘 Guia de Padrões de Código (Coding Standards) — .NET

Este documento estabelece as diretrizes de desenvolvimento para os projetos **.NET** do nosso time. O objetivo principal é garantir consistência, legibilidade, facilidade de manutenção e alta performance em todo a base de código.

---

## 📋 Sumário
1. [Convenções de Nomenclatura](#1-convenções-de-nomenclatura)
2. [Estrutura do Projeto e Organização de Arquivos](#2-estrutura-do-projeto-e-organização-de-arquivos)
3. [Uso do C# Moderno](#3-uso-do-c-moderno)
4. [Tratamento de Erros e Exceções](#4-tratamento-de-erros-e-exceções)
5. [Injeção de Dependência](#5-injeção-de-dependência)
6. [Programação Assíncrona e Performance](#6-programação-assíncrona-e-performance)
7. [Ferramentas de Automação e Linting](#7-ferramentas-de-automação-e-linting)

---

## 1. Convenções de Nomenclatura

## Idioma e linguagem do domínio

- Utilize português, sem acentos, para os conceitos do negócio:
  Projeto, Quadro, Bloco, Personagem e PontoChave.
- Utilize os mesmos termos definidos em contexto.md e nas especificações.
- Preserve nomes técnicos e de frameworks:
  CancellationToken, DbContext, ILogger, Task e PageModel.
- Utilize o sufixo Async nos métodos assíncronos.
- Exemplos: CriarQuadroAsync, projetoId e IPersonagemRepository.
- Não crie Commands, Events ou outros padrões apenas porque aparecem
  nos exemplos deste guia. Sua adoção depende da arquitetura definida.

| Elemento | Convenção | Exemplo |
| :--- | :--- | :--- |
| **Classes, Structs, Enums, Records** | `PascalCase` | `CustomerRepository`, `OrderStatus` |
| **Interfaces** | `IPascalCase` (Prefixo `I`) | `ICustomerRepository`, `IEmailService` |
| **Métodos e Propriedades** | `PascalCase` | `CalculateTotal()`, `CreatedAt` |
| **Métodos Assíncronos** | `PascalCaseAsync` (Sufixo `Async`) | `GetByIdAsync()`, `SaveOrderAsync()` |
| **Parâmetros e Variáveis Locais** | `camelCase` | `totalAmount`, `customerId` |
| **Campos Privados (Fields)** | `_camelCase` (Underline + `camelCase`) | `private readonly ICustomerRepository _repository;` |
| **Constantes** | `PascalCase` | `private const int MaxRetryCount = 3;` |
| **DTOs, Commands, Events** | `PascalCase` + Sufixo explicativo | `CreateUserCommand`, `UserCreatedEvent`, `UserResponseDto` |

### Regras Adicionais de Nomenclatura
* **Enums:** Use singular para Enums comuns (`OrderStatus`) e plural apenas quando usar o atributo `[Flags]` (`UserPermissions`).
* **Booleanos:** Prefira nomes que sugiram verdadeiro/falso: `IsActive`, `HasPermission`, `CanProcess`.

---

## 2. Estrutura do Projeto e Organização de Arquivos

## Construtores e dependências

- Utilize construtores explícitos e campos private readonly
  para dependências injetadas.
- Primary constructors podem ser utilizados quando houver uma
  convenção existente no projeto que justifique sua adoção.
- Entidades e Value Objects devem controlar sua criação
  e impedir estados inválidos.

### 2.1 File-scoped Namespaces
Use **File-scoped Namespaces** para reduzir a indentação desnecessária no arquivo:

```csharp
// ❌ Evite
namespace MyProject.Domain.Services
{
    public class OrderService { }
}

// ✅ Prefira
namespace MyProject.Domain.Services;

public class OrderService { }
```

### 2.2 Um Tipo por Arquivo (One Type Per File)
* Cada arquivo `.cs` deve conter apenas uma classe, interface ou enum.
* **Exceção:** DTOs/Records pequenos e estritamente relacionados ao escopo de um comando/query podem coexistir no mesmo arquivo se acordado pelo time.

### 2.3 Organização Interna de uma Classe
Mantenha os membros organizados na seguinte sequência:

1. Campos privados e constantes (`private readonly`, `private const`)
2. Construtor(es)
3. Propriedades públicas/internas
4. Métodos públicos
5. Métodos privados/auxiliares

```csharp
public class PaymentService
{
    private readonly IPaymentGateway _paymentGateway;
    private const int TimeoutInSeconds = 30;

    public PaymentService(IPaymentGateway paymentGateway)
    {
        _paymentGateway = paymentGateway;
    }

    public decimal MinimumAmount { get; set; }

    public async Task<PaymentResult> ProcessAsync(PaymentRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);
        return await _paymentGateway.PayAsync(request, cancellationToken);
    }

    private void ValidateRequest(PaymentRequest request)
    {
        // Regras locais de validação
    }
}
```

---

## 3. Uso do C# Moderno

### 3.1 Primary Constructors (.NET 8+)
Prefira o uso de Primary Constructors para injeção de dependência em serviços e repositórios:

```csharp
// ✅ Prefira (.NET 8+)
public class OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
{
    public async Task ProcessOrderAsync(Guid orderId)
    {
        logger.LogInformation("Processing order {OrderId}", orderId);
        // ...
    }
}
```

### 3.2 Nullable Reference Types
O projeto deve manter `<Nullable>enable</Nullable>` ativado no `.csproj`.
* Trate alertas de *nullability* explicitamente.
* Utilize o operador `?` para indicar quando um valor pode ser nulo.
* Evite o uso indiscriminado do operador `!` (*null-forgiving operator*).

### 3.3 Uso de `var`
Use `var` quando o tipo do lado direito da atribuição for explícito. Defina o tipo de forma legível nos demais casos:

```csharp
// ✅ Bom uso de var
var user = new User();
var users = new List<User>();

// ❌ Evite (dificulta a leitura sem a IDE)
var result = _repository.ExecuteCustomQuery();

// ✅ Prefira
QueryResult result = _repository.ExecuteCustomQuery();
```

### 3.4 Pattern Matching e Switch Expressions
Prefira `switch expressions` a blocos `switch` ou `if/else` extensos:

```csharp
// ✅ Prefira
public decimal GetDiscount(CustomerType type) => type switch
{
    CustomerType.VIP => 0.20m,
    CustomerType.Regular => 0.05m,
    _ => 0.0m
};
```

### 3.5 Records para DTOs e Value Objects

## Records, entidades e Value Objects

- Prefira records para DTOs que representam dados de entrada ou saída.
- Não considere um record automaticamente imutável.
- Utilize classes para entidades que possuem identidade e ciclo de vida.
- Value Objects devem possuir validação e igualdade por valor.
- Ao utilizar records para Value Objects, impeça que inicializadores
  ou expressões with contornem suas regras de validação.
- Não exponha coleções internas mutáveis de entidades e agregados.

Use `record` (ou `record struct`) para objetos imutáveis de transferência de dados:

```csharp
public record CreateUserRequest(string Name, string Email);
```

---

## 4. Tratamento de Erros e Exceções

## Erros e exceções

- Utilize Result ou Result<T> para falhas esperadas de negócio
  e validações previstas pelos casos de uso.
- Utilize uma única implementação desses tipos no projeto.
- Antes de criar essa implementação ou adicionar uma biblioteca,
  consulte a decisão registrada em arquitetura.md.
- Exceções podem representar falhas inesperadas ou violações
  de contratos de programação.
- Não utilize Exception genérica para representar erros de negócio.
- Não capture exceções apenas para relançá-las.
- Ao relançar uma exceção capturada, utilize throw;
  para preservar a pilha original.
- Não converta cancelamentos solicitados em erros internos.
- Registre falhas inesperadas na fronteira responsável pelo tratamento,
  evitando logs duplicados da mesma falha.
- Não registre senhas, tokens ou conteúdo privado das obras nos logs.

### 4.1 Result Pattern vs Exceções
* **Exceções (`throw`):** Reservadas exclusivamente para falhas **inesperadas ou catastróficas** (ex.: banco de dados offline, falha de rede, arquivo não encontrado).
* **Result Pattern / Domain Errors:** Utilize tipos de retorno explicitos (ex.: `Result<T>`) para erros de negócio e validações previsíveis.

```csharp
// ❌ Evite usar exceções para controle de fluxo de negócio
if (user == null)
    throw new Exception("Usuário não encontrado.");

// ✅ Prefira retornar um objeto de resultado
if (user is null)
    return Result.Failure(UserErrors.NotFound);
```

### 4.2 Logging e Exceções Capturadas
Sempre utilize logs estruturados (com placeholders) em vez de interpolação de strings:

```csharp
// ❌ Evite
_logger.LogError($"Erro ao processar pedido {orderId}: {ex.Message}");

// ✅ Prefira
_logger.LogError(ex, "Erro ao processar pedido {OrderId}", orderId);
```

---

## 5. Injeção de Dependência

## Injeção de dependência

- Registre dependências na composição da aplicação.
- Utilize Scoped para DbContext, repositórios e serviços que
  compartilham a unidade de trabalho da requisição.
- Utilize Transient quando uma nova instância por resolução
  for apropriada.
- Utilize Singleton somente para serviços adequados ao
  compartilhamento concorrente durante a vida da aplicação.
- Não injete dependências Scoped diretamente em Singletons.
- Não utilize Service Locator nos casos de uso ou no domínio.

* **Declaração de Lifetimes:** Defina explicitamente os tempos de vida adequados ao registrar dependências na camada de composição da aplicação (`Program.cs` / Dependency Injection Extensions):
  * `AddTransient`: Serviços sem estado (*stateless*) e leves.
  * `AddScoped`: Serviços associados a um escopo de requisição (padrão em APIs Web, repositórios, DbContext).
  * `AddSingleton`: Serviços compartilhados globalmente e thread-safe (ex.: cache em memória, clientes HTTP reutilizáveis).
* **Proibido Service Locator:** Nunca injete `IServiceProvider` diretamente em classes de negócio para resolver dependências em runtime.

---

## 6. Programação Assíncrona e Performance

## Coleções e consultas

- Prefira IReadOnlyList<T> quando a ordem do resultado for relevante.
- Prefira IReadOnlyCollection<T> para coleções materializadas
  sem acesso por índice.
- Utilize IEnumerable<T> quando a enumeração fizer parte
  explícita do contrato.
- Não exponha IQueryable<T> nas portas consumidas pela aplicação
  ou pela interface web.
- Materialize consultas antes de encerrar o escopo do DbContext.
- Utilize AsNoTracking em consultas de leitura que retornem entidades
  e não precisem de rastreamento.
- Projete consultas de listagem diretamente para os dados necessários.
- Não execute operações paralelas na mesma instância de DbContext.

### 6.1 `async`/`await` e Deadlocks
* **Nunca** use `.Result` ou `.Wait()`. Isso pode causar bloqueio de threads (*thread starvation*) e deadlocks.
* Evite `async void` a todo custo (utilize apenas em *event handlers* de UI, caso aplicável).

### 6.2 CancellationToken
Passe o `CancellationToken` ao longo de toda a cadeia de chamadas assíncronas:

```csharp
public async Task<UserResponseDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
{
    var user = await _dbContext.Users
        .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        
    return user?.ToDto();
}
```

### 6.3 Performance de Coleções
* Ao retornar coleções em métodos públicos de leitura, prefira `IReadOnlyCollection<T>` ou `IEnumerable<T>`.
* Para consultas do Entity Framework onde não haverá alteração no estado dos objetos, utilize `.AsNoTracking()`.


