namespace EscribaApp.Dominio.Resultados;

public sealed class Resultado<T> : Resultado where T : notnull
{
    private readonly T? _valor;

    private Resultado(T? valor, Erro? erro) : base(erro)
    {
        _valor = valor;
    }

    public T Valor => FoiSucesso && _valor is not null
        ? _valor
        : throw new InvalidOperationException("Um resultado de falha não possui valor.");

    public static Resultado<T> Sucesso(T valor)
    {
        ArgumentNullException.ThrowIfNull(valor);
        return new(valor, null);
    }

    public new static Resultado<T> Falha(Erro erro)
    {
        ArgumentNullException.ThrowIfNull(erro);
        return new(default, erro);
    }
}
