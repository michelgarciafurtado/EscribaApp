# Regras de negócio — modelagem do domínio

## Vocabulário

- Utilizar Personagem para os atores fictícios e PontoChave para fatos-chave.
- Tipos de projeto: Filme, Novela, Livro, HQ, Video, PecaTeatral e Outro.
- Projeto admite múltiplos gêneros. O nome do gênero é armazenado sem espaços
  externos e em maiúsculas invariantes para igualdade sem diferenciar caixa.
- Objetivos narrativos são comparados pelo texto sem espaços externos,
  diferenciando maiúsculas e minúsculas.

## Criação e alterações

- Nome obrigatório para Projeto, Personagem e Quadro.
- Descrição obrigatória para PontoChave, objetivo de Bloco e ObjetivoNarrativo.
- Demais textos descritivos podem começar vazios para permitir rascunhos.
- Identidades são imutáveis e geradas pelo domínio.
- As alterações passam pela raiz Projeto. Uma falha prevista não altera o estado.
- Associações somente entre elementos cadastrados no mesmo projeto, sem duplicação.
- Nomes iguais de entidades não são proibidos: a identidade é o identificador.

## Associações e remoção

- Um quadro pertence a zero ou um bloco; associá-lo a outro bloco o transfere.
- Um ponto-chave pode participar de vários quadros e ter um personagem responsável opcional.
- O responsável pelo ponto-chave não precisa estar associado a todos os quadros
  em que o ponto aparece, pois pode ser referido sem participar da cena.
- Personagem associado a quadro ou responsável por ponto-chave não pode ser removido.
- Ponto-chave associado a quadro não pode ser removido.
- Bloco com quadros não pode ser removido; antes, transferir ou desassociar seus quadros.
- Quadro com ligações de entrada ou saída não pode ser removido antes da desconexão.
  Remover um quadro desconectado também elimina suas associações e objetivos internos.

## Sequências e ramificações

- Blocos e pontos-chave recebem sequência contígua a partir de 1.
- Reordenações recebem todos os identificadores atuais exatamente uma vez.
  Listas incompletas, repetidas, nulas ou com identificadores externos são rejeitadas.
- Remover bloco ou ponto-chave compacta as sequências dos elementos restantes.
- A sequência de ponto-chave é sua ordem planejada no projeto, não uma posição
  obrigatória em cada caminho da narrativa.
- LigacaoQuadros contém QuadroOrigemId, QuadroDestinoId e Ordem.
- Origem e destino devem existir no projeto, ser diferentes e não formar um ciclo.
- Não pode haver duas ligações com o mesmo par origem/destino.
- Ordem é a prioridade entre sucessores da mesma origem, contígua a partir de 1.
- Novas ligações entram ao final; `ReordenarSucessores` altera as prioridades
  mediante a lista completa dos sucessores diretos. Desconexões compactam a ordem.
- Objetos de valor são substituídos durante reordenações, nunca modificados.
- Quadros isolados são simultaneamente iniciais e finais. Múltiplas entradas,
  saídas e componentes desconectados são permitidos durante o planejamento.
- `ExisteCaminho` considera um caminho de comprimento zero para um quadro existente
  até ele mesmo; retorna falso quando origem ou destino não existem no projeto.
- Flashbacks podem ser descritos nas cenas; não autorizam ciclos na escaleta.
