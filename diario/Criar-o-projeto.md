# Criar o projeto

## Resumo da interação — 09/11/2026

O usuário solicitou a leitura da tarefa `.agent/tarefas/Criar o projeto.md`
e das referências do `AGENTS.MD`, com elaboração de um plano e implementação
somente após seu aceite. Foram consultados os documentos de contexto,
arquitetura e padrões de código, além do README. Os arquivos de regras de
negócio e testes estavam vazios, e ainda não havia código da aplicação.

O plano propôs quatro projetos .NET 10, uma solução `.slnx`, referências
voltadas ao núcleo e os pacotes de Identity e PostgreSQL na Infra. Foram
identificadas divergências entre a menção a WebApi e a exigência de Razor
Pages, e entre a estrutura antiga do README e a estrutura solicitada na tarefa.

O usuário autorizou a implementação com Razor Pages e o uso da branch atual
`feat-Criar-Projeto`. Após uma confirmação sobre a disposição das pastas,
definiu `app/src` e `app/adapters`, com `app` no mesmo nível de `.agent`.

Foram criados os projetos `EscribaApp.Dominio`, `EscribaApp.Aplicacao`,
`EscribaApp.Infra` e `EscribaApp.Web`. As três primeiras camadas são
bibliotecas de classes; Web utiliza Razor Pages. No domínio, foram criadas
as pastas `Agregados`, `Entidades` e `ObjetosValor`, com arquivos `.gitkeep`
para preservá-las no Git.

As referências ficaram organizadas da seguinte forma:

- Aplicacao referencia Dominio.
- Infra referencia Aplicacao e Dominio.
- Web referencia Aplicacao e Infra.
- Dominio não referencia outros projetos nem pacotes externos.

Na Infra, foram adicionados `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
10.0.14 e `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.2. Também foram removidas
as classes de exemplo das bibliotecas, ajustados os textos da interface para
português e adicionados `.gitignore` e instruções de execução ao README.

A restauração de pacotes exigiu acesso ao NuGet fora do sandbox. A compilação
terminou sem erros ou avisos. Após a primeira tentativa HTTP expirar no
ambiente restrito, a verificação fora do sandbox confirmou status 200,
idioma `pt-BR` e o conteúdo da página inicial.

As alterações preexistentes do usuário nos documentos de `.agent` foram
preservadas. Não foram implementados casos de uso, entidades de negócio,
autenticação, contexto de banco ou migrações. Nenhum commit foi criado.

## Decisões

- Solução `.slnx` com quatro projetos .NET 10.
- Núcleo em `app/src` e adaptadores em `app/adapters`, com `app` ao lado de `.agent`.
- Razor Pages conforme a tarefa e o contexto, apesar da menção a WebApi na especificação.
- Domínio independente de frameworks e bibliotecas externas.
- Identity com EF Core e provedor PostgreSQL restritos ao projeto Infra.
- Sem implementação de regras de negócio, autenticação ou banco nesta etapa.

## Aprendizado

As referências entre projetos apontam para o núcleo. A interface pode conhecer
Infra para compor dependências, sem transferir regras de negócio aos adaptadores.

## Verificação

- Restauração de pacotes concluída.
- Compilação dos quatro projetos sem erros ou avisos.
- Página inicial verificada por HTTP: status 200, idioma `pt-BR` e conteúdo sobre escaleta.
- Referências dos projetos conferidas; Dominio sem `ProjectReference` ou `PackageReference`.
- A verificação HTTP precisou executar fora do sandbox. Usando apenas HTTP, sem
  perfil de execução, o middleware registrou ausência de porta HTTPS; para uso
  local, os perfis gerados pelo template estão disponíveis.
