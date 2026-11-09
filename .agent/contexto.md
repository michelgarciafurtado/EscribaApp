# Contexto do projeto — EscribaWeb

## Visão geral

O EscribaWeb é uma aplicação web destinada a ajudar escritores
e roteiristas a planejar e organizar suas histórias utilizando
a técnica da escaleta, representada como uma escada narrativa.

Cada projeto representa uma obra. Os degraus dessa escada são
chamados de Quadros e organizam as cenas ou capítulos da narrativa.

## Objetivo

Oferecer um ambiente para estruturar histórias, organizar sua
sequência narrativa e acompanhar personagens, objetivos e pontos
importantes para a coerência da trama.

## Público-alvo

Escritores, roteiristas e autores de histórias em quadrinhos.

## Tecnologias e abordagem

- Plataforma: .NET.
- Tipo de aplicação: web.
- Arquitetura: Hexagonal.
- Modelagem do domínio: princípios de Domain-Driven Design (DDD).
- Banco de dados: PostgreSQL.
- Persistência: Entity Framework Core.
- Interface: razor pages

## Conceitos do negócio

### Projeto

Representa a obra que o autor está planejando.

Um projeto possui:

- Nome
- Tipo: Filme, Novela, Livro, HQ, Video, PecaTeatral ou Outro.
- Descrição.
- Gêneros (múltiplos).
- Personagens cadastrados.
- Lista de Pontos Chave.
- Lista de Quadros que compõem a escaleta.

### Escaleta

É a organização da narrativa em uma sequência de Quadros.

A metáfora da escada representa a progressão da história:
cada Quadro corresponde a um degrau dessa construção.

### Quadro

É a unidade de planejamento narrativo do projeto.

Pode representar uma cena ou um capítulo, conforme a organização
adotada pelo autor.

Um Quadro possui:

- Ligações com Quadros antecessores e sucessores, permitindo ramificações e convergências sem ciclos.
- Referências aos Pontos-chave, quando houver.
- Indicação do Bloco ao qual pertence.
- Lista de personagens envolvidos.
- Descrição da cena ou do capítulo.
- Objetivos narrativos.

### Bloco

Representa um agrupamento de Quadros dentro de um projeto que representa um momento específico da trama.

Cada bloco possui:

- Sequencia.
- Descricao do objetvo.
- lista de quadros que pertencem.

### Personagem

Representa uma figura fictícia que participa da história e deve ser cadastrado pelo autor.

Um personagem possui:

- Nome.
- Biotipo.
- Personalidade.
- Principais feitos na trama.

Os personagens são cadastrados no projeto e podem ser associados
aos Quadros em que participam.

### Ponto-chave

Representa uma informação, acontecimento ou detalhe que não
pode ser esquecido para que a trama mantenha sua coerência.

Os pontos-chave são criados no projeto e registrados nos Quadros.

Um ponto chave possui:

- número de sequencia que deve aparecer
- descricao do que deve acontecer
- personagem que realiza o ato, quando houver

### Objetivo narrativo

Descreve o que o autor pretende alcançar com um Quadro
na construção da história.

## Funcionalidades previstas

- Cadastrar projetos dos tipos Filme, Novela, Livro, HQ, Video, PecaTeatral e Outro.
- Cadastrar os pontos Chave do Projeto
- Registrar a descrição e os gêneros de cada projeto.
- Cadastrar personagens de um projeto.
- Criar e organizar Pontos chave.
- Criar e organizar Quadros na escaleta.
- Definir antecessores e sucessores de cada Quadro e a prioridade de suas alternativas de saída.
- Associar Quadros a Blocos.
- Associar personagens aos Quadros.
- Associar pontos chave aos Quadros.
- Registrar descrições, objetivos e pontos-chave dos Quadros.
