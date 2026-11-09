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
  - Nome, Gênero (múltiplos), Lista de personagens, Lista de fatos-chave.
  - Criação de ramificações de quadros e organização em blocos.
- **Cadastro de Personagens**:
  - Nome, Descrição, Biotipo, Contribuições no enredo.
- **Cadastro de Fatos-Chave**:
  - Sequência, Descrição.
- **Cadastro de Quadros**:
  - Sucessor e antecessor (suporte a múltiplos antecessores/sucessores), Nome, Personagens e Fatos-chave relacionados.
  - *Nota:* Personagens e fatos-chave devem estar previamente cadastrados para serem associados.

---

## ⚙️ Contexto Técnico e Arquitetural

- **Metodologia:** SDD (Spec Driven Design) com apoio de IA no VS Code.
- **Arquitetura:** DDD e **Arquitetura Hexagonal** dividida entre `core` e `adapters`.
- **Estrutura de Diretórios:**
  - `.agent/tarefas`: Armazenamento das tarefas de desenvolvimento.
  - `src`: Código-fonte da aplicação (Hexagonal).
  - `diario`: Registro de experiências e aprendizados (criado após a primeira tarefa).

---

## 🔄 Metodologia de Trabalho

- Cada tarefa é executada em uma **branch própria** com o mesmo nome da tarefa.
- Após a verificação e aprovação, é feito o *merge* nas branches **`develop`** e **`main`**.
- Interferência mínima de codificação manual (foco em ajustes pontuais).
- Registro de experiências e aprendizados após cada tarefa realizada.
