namespace EscribaApp.Dominio.Entidades;

public sealed class PontoChave
{
    internal PontoChave(int sequencia, string descricao)
    {
        Id = Guid.NewGuid();
        Sequencia = sequencia;
        Descricao = descricao;
    }

    public Guid Id { get; }
    public int Sequencia { get; private set; }
    public string Descricao { get; private set; }
    public Guid? PersonagemResponsavelId { get; private set; }

    internal void AlterarDescricao(string descricao) => Descricao = descricao;
    internal void AlterarSequencia(int sequencia) => Sequencia = sequencia;
    internal void DefinirResponsavel(Guid? personagemId) => PersonagemResponsavelId = personagemId;
}
