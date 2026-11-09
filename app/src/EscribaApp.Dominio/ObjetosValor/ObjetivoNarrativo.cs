using EscribaApp.Dominio.Resultados;

namespace EscribaApp.Dominio.ObjetosValor;

public sealed record ObjetivoNarrativo
{
    private ObjetivoNarrativo(string descricao)
    {
        Descricao = descricao;
    }

    public string Descricao { get; }

    public static Resultado<ObjetivoNarrativo> Criar(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            return Resultado<ObjetivoNarrativo>.Falha(new Erro("Objetivo.DescricaoObrigatoria", "Informe o objetivo narrativo."));

        return Resultado<ObjetivoNarrativo>.Sucesso(new ObjetivoNarrativo(descricao.Trim()));
    }
}
