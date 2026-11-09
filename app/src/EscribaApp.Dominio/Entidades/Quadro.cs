using EscribaApp.Dominio.ObjetosValor;

namespace EscribaApp.Dominio.Entidades;

public sealed class Quadro
{
    private readonly List<Guid> _personagensIds = [];
    private readonly List<Guid> _pontosChaveIds = [];
    private readonly List<ObjetivoNarrativo> _objetivosNarrativos = [];

    internal Quadro(string nome, string descricao)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
    }

    public Guid Id { get; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public Guid? BlocoId { get; private set; }
    public IReadOnlyCollection<Guid> PersonagensIds => _personagensIds.AsReadOnly();
    public IReadOnlyCollection<Guid> PontosChaveIds => _pontosChaveIds.AsReadOnly();
    public IReadOnlyList<ObjetivoNarrativo> ObjetivosNarrativos => _objetivosNarrativos.AsReadOnly();

    internal void AlterarDados(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
    }

    internal void AssociarPersonagem(Guid personagemId) => _personagensIds.Add(personagemId);
    internal void DesassociarPersonagem(Guid personagemId) => _personagensIds.Remove(personagemId);
    internal void AssociarPontoChave(Guid pontoChaveId) => _pontosChaveIds.Add(pontoChaveId);
    internal void DesassociarPontoChave(Guid pontoChaveId) => _pontosChaveIds.Remove(pontoChaveId);
    internal void DefinirBloco(Guid? blocoId) => BlocoId = blocoId;
    internal void AdicionarObjetivo(ObjetivoNarrativo objetivo) => _objetivosNarrativos.Add(objetivo);
    internal void RemoverObjetivo(ObjetivoNarrativo objetivo) => _objetivosNarrativos.Remove(objetivo);
}
