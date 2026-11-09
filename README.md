# EscribaApp

> Aplicação web para estruturação de obras literárias e audiovisuais através da técnica de escaleta, desenvolvida com metodologia Spec Driven Design (SDD) e assistência de IA.

---

## 🎯 Introdução e Objetivo

Desenvolver uma aplicação para exercitar e consolidar conceitos e competências de desenvolvimento usando técnicas do **SDD (Spec Driven Design)**, com apoio de IA como agente no **Visual Studio Code**.

---

## 🏢 Contexto de Negócio

O **EscribaApp** destina-se a ajudar escritores e diretores a estruturarem suas obras através da técnica da **escaleta**.

Cada degrau representa uma cena ou capítulo específico da obra. Cada quadro possui relação com seu antecessor e sucessor, registrando quem participa dele, seu objetivo e descrição, facilitando ao autor manter a coerência lógica dentro da perspectiva temporal da obra.

### Requisitos Funcionais
- **Cadastro de Projetos** (Livro, filme, vídeo, novela, peça teatral e outros):
  - Nome, Gêneros (múltiplos), Lista de personagens, Lista de pontos-chave.
  - Criação de ramificações de quadros e organização em blocos.
- **Cadastro de Personagens**:
  - Nome, Descrição, Biotipo, Contribuições no enredo.
- **Cadastro de Pontos-chave**:
  - Sequência, Descrição.
- **Cadastro de Quadros**:
  - Ligações com múltiplos antecessores e sucessores, Nome, Personagens e Pontos-chave relacionados.
  - *Nota:* Personagens e pontos-chave devem estar previamente cadastrados para serem associados.

---

## ⚙️ Contexto Técnico e Arquitetural

- **Metodologia:** SDD (Spec Driven Design) com apoio de IA no VS Code.
- **Arquitetura:** DDD e **Arquitetura Hexagonal**, com o núcleo em `app/src` e adaptadores em `app/adapters`.
- **Estrutura de Diretórios:**
  - `.agent/tarefas`: Armazenamento das tarefas de desenvolvimento.
  - `app/src`: Bibliotecas de domínio e aplicação.
  - `app/adapters`: Infraestrutura e interface Razor Pages.
  - `testes`: Testes de domínio com NUnit.
  - `diario`: Registro de experiências e aprendizados (criado após a primeira tarefa).

---

## 🔄 Metodologia de Trabalho

- Cada tarefa é executada em uma **branch própria** com o mesmo nome da tarefa.
- Após a verificação e aprovação, é feito o *merge* nas branches **`develop`** e **`main`**.
- Interferência mínima de codificação manual (foco em ajustes pontuais).
- Registro de experiências e aprendizados após cada tarefa realizada.

## Estrutura da solução

```text
EscribaApp.slnx
app/
├── src/
│   ├── EscribaApp.Dominio/
│   │   ├── Agregados/
│   │   ├── Entidades/
│   │   └── ObjetosValor/
│   └── EscribaApp.Aplicacao/
└── adapters/
    ├── EscribaApp.Infra/
    └── EscribaApp.Web/
```

Dominio não possui referências a outros projetos ou pacotes externos.
Aplicacao referencia Dominio. Infra referencia Aplicacao e Dominio.
Web referencia Aplicacao e Infra para composição das dependências.

Infra inclui `Microsoft.AspNetCore.Identity.EntityFrameworkCore` para integração
do Identity com EF Core e `Npgsql.EntityFrameworkCore.PostgreSQL` para PostgreSQL.
Esta estrutura inicial ainda não configura autenticação, contexto de banco ou migrações.

## Execução local

Requer o SDK .NET 10. Execute os comandos na pasta que contém `EscribaApp.slnx`:

```powershell
dotnet restore EscribaApp.slnx
dotnet build EscribaApp.slnx --no-restore
dotnet run --project app/adapters/EscribaApp.Web --launch-profile http
```

Acesse o endereço exibido no terminal. A página inicial funciona sem banco de dados.
Não registre credenciais nos arquivos; quando necessário, utilize variáveis de
ambiente ou User Secrets no desenvolvimento.

## Domínio e testes

`Projeto` controla personagens, quadros, blocos, pontos-chave e gêneros.
As ramificações são representadas por `LigacaoQuadros`, um objeto de valor
imutável com origem, destino e prioridade da saída. O agregado impede ciclos
e associações entre obras diferentes. Antecessores, sucessores e ordenação
topológica são consultados a partir dessa coleção única de ligações.

```powershell
dotnet test testes/EscribaApp.Dominio.Tests/EscribaApp.Dominio.Tests.csproj
```

As regras detalhadas estão em `.agent/regras-negocio.md`. As entidades ainda
não possuem mapeamento de persistência ou integração com a interface Web.
