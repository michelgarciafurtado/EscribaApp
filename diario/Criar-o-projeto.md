# Criar o projeto

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
