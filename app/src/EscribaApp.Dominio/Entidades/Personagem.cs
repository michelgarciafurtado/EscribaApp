namespace EscribaApp.Dominio.Entidades;

public sealed class Personagem
{
    internal Personagem(string nome, string descricao, string biotipo, string personalidade, string principaisFeitos)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
        Biotipo = biotipo;
        Personalidade = personalidade;
        PrincipaisFeitos = principaisFeitos;
    }

    public Guid Id { get; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public string Biotipo { get; private set; }
    public string Personalidade { get; private set; }
    public string PrincipaisFeitos { get; private set; }

    internal void AlterarDados(string nome, string descricao, string biotipo, string personalidade, string principaisFeitos)
    {
        Nome = nome;
        Descricao = descricao;
        Biotipo = biotipo;
        Personalidade = personalidade;
        PrincipaisFeitos = principaisFeitos;
    }
}
