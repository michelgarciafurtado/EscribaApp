using EscribaApp.Dominio.ObjetosValor;
using EscribaApp.Dominio.Resultados;
using NUnit.Framework;

namespace EscribaApp.Dominio.Tests;

[TestFixture]
public class ObjetosValorTests
{
    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void TextosObrigatoriosNaoAceitamValoresVazios(string texto)
    {
        Assert.Multiple(() =>
        {
            Assert.That(Genero.Criar(texto).FoiFalha, Is.True);
            Assert.That(ObjetivoNarrativo.Criar(texto).FoiFalha, Is.True);
        });
    }

    [Test]
    public void GenerosSaoIguaisSemDiferenciarMaiusculasOuEspacosExternos()
    {
        Genero primeiro = Genero.Criar("  Drama ").Valor;
        Genero segundo = Genero.Criar("drama").Valor;
        Assert.That(new HashSet<Genero> { primeiro, segundo }, Has.Count.EqualTo(1));
    }

    [Test]
    public void ObjetivosSaoComparadosPeloTextoNormalizado()
    {
        Assert.That(ObjetivoNarrativo.Criar(" Revelar o segredo ").Valor,
            Is.EqualTo(ObjetivoNarrativo.Criar("Revelar o segredo").Valor));
    }

    [Test]
    public void LigacoesValidamIdentificadoresAutorreferenciaEOrdem()
    {
        Guid origem = Guid.NewGuid();
        Guid destino = Guid.NewGuid();
        Assert.Multiple(() =>
        {
            Assert.That(LigacaoQuadros.Criar(Guid.Empty, destino, 1).FoiFalha, Is.True);
            Assert.That(LigacaoQuadros.Criar(origem, Guid.Empty, 1).FoiFalha, Is.True);
            Assert.That(LigacaoQuadros.Criar(origem, origem, 1).Erro?.Codigo, Is.EqualTo("Ligacao.Autorreferencia"));
            Assert.That(LigacaoQuadros.Criar(origem, destino, 0).FoiFalha, Is.True);
            Assert.That(LigacaoQuadros.Criar(origem, destino, -1).FoiFalha, Is.True);
            Assert.That(LigacaoQuadros.Criar(origem, destino, 1).Valor,
                Is.EqualTo(LigacaoQuadros.Criar(origem, destino, 1).Valor));
            Assert.That(LigacaoQuadros.Criar(origem, destino, 1).Valor,
                Is.Not.EqualTo(LigacaoQuadros.Criar(origem, destino, 2).Valor));
        });
    }

    [Test]
    public void FalhaNaoDisponibilizaValorECarregaErro()
    {
        var erro = new Erro("Teste.Falha", "Falha prevista.");
        Resultado<int> resultado = Resultado<int>.Falha(erro);
        Assert.Multiple(() =>
        {
            Assert.That(resultado.FoiFalha, Is.True);
            Assert.That(resultado.Erro, Is.EqualTo(erro));
            Assert.Throws<InvalidOperationException>(() => _ = resultado.Valor);
            Assert.That(Resultado<int>.Sucesso(0).Valor, Is.Zero);
        });
    }
}
