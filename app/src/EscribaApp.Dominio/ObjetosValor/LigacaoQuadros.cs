using EscribaApp.Dominio.Resultados;

namespace EscribaApp.Dominio.ObjetosValor;

public sealed record LigacaoQuadros
{
    private LigacaoQuadros(Guid quadroOrigemId, Guid quadroDestinoId, int ordem)
    {
        QuadroOrigemId = quadroOrigemId;
        QuadroDestinoId = quadroDestinoId;
        Ordem = ordem;
    }

    public Guid QuadroOrigemId { get; }
    public Guid QuadroDestinoId { get; }
    public int Ordem { get; }

    public static Resultado<LigacaoQuadros> Criar(Guid quadroOrigemId, Guid quadroDestinoId, int ordem)
    {
        if (quadroOrigemId == Guid.Empty || quadroDestinoId == Guid.Empty)
            return Resultado<LigacaoQuadros>.Falha(new Erro("Ligacao.IdentificadorInvalido", "Informe os identificadores dos quadros."));
        if (quadroOrigemId == quadroDestinoId)
            return Resultado<LigacaoQuadros>.Falha(new Erro("Ligacao.Autorreferencia", "Um quadro não pode ser ligado a ele mesmo."));
        if (ordem < 1)
            return Resultado<LigacaoQuadros>.Falha(new Erro("Ligacao.OrdemInvalida", "A ordem deve ser maior que zero."));

        return Resultado<LigacaoQuadros>.Sucesso(new LigacaoQuadros(quadroOrigemId, quadroDestinoId, ordem));
    }
}
