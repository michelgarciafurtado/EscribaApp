namespace EscribaApp.Dominio.Resultados;

public class Resultado
{
    private protected Resultado(Erro? erro)
    {
        Erro = erro;
    }

    public bool FoiSucesso => Erro is null;
    public bool FoiFalha => !FoiSucesso;
    public Erro? Erro { get; }

    public static Resultado Sucesso() => new(null);

    public static Resultado Falha(Erro erro)
    {
        ArgumentNullException.ThrowIfNull(erro);
        return new(erro);
    }
}
