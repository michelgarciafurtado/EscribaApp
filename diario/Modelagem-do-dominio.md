# Modelagem do domínio — 09/11/2026

## Resumo da interação

O usuário pediu um plano antes de qualquer alteração, incluindo o conteúdo e a
localização das classes. Foram consultados AGENTS.MD, a tarefa de modelagem,
contexto, arquitetura, padrões de código e README, além do domínio existente.

O plano apresentou Projeto como raiz do agregado, quatro entidades e três
objetos de valor. Também apontou divergências sobre atores/personagens,
fatos-chave/pontos-chave, gêneros, tipos de obra e multiplicidade das ligações.
O usuário aprovou o vocabulário sugerido e determinou NUnit em uma pasta
`testes` no mesmo nível de `app`.

Foram comparadas duas alternativas para as ramificações: uma coleção de
LigacaoQuadros no agregado e listas de referências em Quadro. O usuário
aprovou a primeira alternativa antes da implementação.

## Implementação

- `Agregados/Projeto.cs`: cadastro, atualização, associações, remoções,
  reordenações e consultas de elementos da obra.
- `Entidades`: Personagem, Quadro, Bloco e PontoChave, com identidades imutáveis
  e alterações internas coordenadas pelo agregado.
- `ObjetosValor`: Genero, ObjetivoNarrativo e LigacaoQuadros, imutáveis e com
  criação validada e igualdade por valor.
- `Enumeracoes/TipoProjeto.cs`: Filme, Novela, Livro, HQ, Video, PecaTeatral e Outro.
- `Resultados`: implementação única de Resultado, Resultado<T> e Erro,
  sem bibliotecas externas no domínio.
- `testes/EscribaApp.Dominio.Tests`: NUnit, NUnit3TestAdapter e
  Microsoft.NET.Test.Sdk, restritos ao projeto de testes e adicionados à solução.

LigacaoQuadros contém origem, destino e prioridade de saída. Projeto impede
autorreferências, duplicidades, ligações entre obras e ciclos. Permite
ramificação, convergência e quadros isolados. Consultas de antecessores,
sucessores, entradas, saídas, alcance e ordenação topológica derivam da coleção
única de ligações. Reordenar substitui os objetos de valor.

Coleções são expostas somente para leitura. Associações usadas bloqueiam
remoções que deixariam referências inválidas. Sequências de blocos,
pontos-chave e saídas são compactadas após remoções ou desconexões.

## Documentação e escopo

Arquitetura, contexto, regras de negócio, orientações de testes e README foram
atualizados com as decisões aprovadas. A implementação ocorreu na branch
`feat-Modelagem-Dominio`. A alteração preexistente do usuário no arquivo da
tarefa foi preservada.

Não foram adicionados persistência, autenticação, casos de uso ou interface para
a modelagem. Nenhum commit ou merge foi realizado.

## Verificação

- 28 testes NUnit aprovados, sem falhas ou testes ignorados.
- Solução completa compilada com zero erros e zero avisos.
- Cobertura de criação e atualização, falhas sem alteração parcial, referências
  externas, remoções protegidas, coleções somente de leitura e igualdade por valor.
- Ramificações e convergências verificadas, incluindo rejeição de ciclos em
  cadeias de 2, 5 e 100 quadros, reordenação e componentes desconectados.
- A restauração inicial apresentou avisos de acesso às fontes configuradas no
  ambiente; a restauração pela origem pública do NuGet fora do sandbox concluiu
  sem esses avisos.

## Aprendizado

Manter uma única representação das ligações evita sincronizar listas de entrada
e saída nos quadros. A raiz do agregado garante a integridade das relações,
enquanto os objetos de valor garantem a validade dos dados de cada ligação.
Uma ordenação topológica respeita precedências, mas não seleciona um caminho
narrativo entre alternativas.
