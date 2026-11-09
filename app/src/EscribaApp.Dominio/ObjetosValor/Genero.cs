using EscribaApp.Dominio.Resultados;

namespace EscribaApp.Dominio.ObjetosValor;

public sealed record Genero
{
    private Genero(string nome)
    {
        Nome = nome;
    }

    public string Nome { get; }

    public static Resultado<Genero> Criar(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Resultado<Genero>.Falha(new Erro("Genero.NomeObrigatorio", "Informe o gênero."));

        return Resultado<Genero>.Sucesso(new Genero(nome.Trim().ToUpperInvariant()));
    }
}
