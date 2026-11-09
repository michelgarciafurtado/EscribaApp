namespace EscribaApp.Dominio.Entidades;

public sealed class Bloco
{
    internal Bloco(int sequencia, string descricaoObjetivo)
    {
        Id = Guid.NewGuid();
        Sequencia = sequencia;
        DescricaoObjetivo = descricaoObjetivo;
    }

    public Guid Id { get; }
    public int Sequencia { get; private set; }
    public string DescricaoObjetivo { get; private set; }

    internal void AlterarObjetivo(string descricaoObjetivo) => DescricaoObjetivo = descricaoObjetivo;
    internal void AlterarSequencia(int sequencia) => Sequencia = sequencia;
}
