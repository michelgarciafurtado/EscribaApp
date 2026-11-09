using EscribaApp.Dominio.Agregados;
using EscribaApp.Dominio.Entidades;
using EscribaApp.Dominio.Enumeracoes;
using EscribaApp.Dominio.ObjetosValor;
using NUnit.Framework;

namespace EscribaApp.Dominio.Tests;

[TestFixture]
public class RamificacoesTests
{
    [Test]
    public void RamificacaoComConvergenciaPreservaAntecessoresSucessoresEOrdemTopologica()
    {
        Projeto projeto = NovoProjeto();
        Quadro inicio = projeto.CriarQuadro("Início").Valor;
        Quadro alternativaA = projeto.CriarQuadro("Alternativa A").Valor;
        Quadro alternativaB = projeto.CriarQuadro("Alternativa B").Valor;
        Quadro fim = projeto.CriarQuadro("Convergência").Valor;
        Assert.That(projeto.ConectarQuadros(inicio.Id, alternativaA.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ConectarQuadros(inicio.Id, alternativaB.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ConectarQuadros(alternativaA.Id, fim.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ConectarQuadros(alternativaB.Id, fim.Id).FoiSucesso, Is.True);

        Assert.Multiple(() =>
        {
            Assert.That(projeto.Ligacoes, Has.Count.EqualTo(4));
            Assert.That(projeto.ObterSucessores(inicio.Id), Is.EqualTo(new[] { alternativaA, alternativaB }));
            Assert.That(projeto.ObterAntecessores(fim.Id), Is.EquivalentTo(new[] { alternativaA, alternativaB }));
            Assert.That(projeto.ObterQuadrosIniciais(), Is.EqualTo(new[] { inicio }));
            Assert.That(projeto.ObterQuadrosFinais(), Is.EqualTo(new[] { fim }));
            Assert.That(projeto.ExisteCaminho(inicio.Id, fim.Id), Is.True);
            Assert.That(projeto.ExisteCaminho(alternativaA.Id, alternativaB.Id), Is.False);
            Assert.That(projeto.ExisteCaminho(fim.Id, inicio.Id), Is.False);
        });
        VerificarOrdemTopologica(projeto);
    }

    [TestCase(2)]
    [TestCase(5)]
    [TestCase(100)]
    public void CicloIndiretoERejeitadoSemModificarLigacoes(int quantidadeQuadros)
    {
        Projeto projeto = NovoProjeto();
        Quadro[] quadros = Enumerable.Range(0, quantidadeQuadros)
            .Select(indice => projeto.CriarQuadro($"Cena {indice}").Valor).ToArray();
        for (int indice = 1; indice < quadros.Length; indice++)
            Assert.That(projeto.ConectarQuadros(quadros[indice - 1].Id, quadros[indice].Id).FoiSucesso, Is.True);

        LigacaoQuadros[] anteriores = projeto.Ligacoes.ToArray();
        Assert.That(projeto.ConectarQuadros(quadros[^1].Id, quadros[0].Id).Erro?.Codigo, Is.EqualTo("Ligacao.Ciclo"));
        Assert.That(projeto.Ligacoes, Is.EqualTo(anteriores));
        VerificarOrdemTopologica(projeto);
    }

    [Test]
    public void AutorreferenciaDuplicidadeEOrdemInvalidaNaoAlteramGrafo()
    {
        Projeto projeto = NovoProjeto();
        Quadro origem = projeto.CriarQuadro("Origem").Valor;
        Quadro destino = projeto.CriarQuadro("Destino").Valor;
        Assert.That(projeto.ConectarQuadros(origem.Id, origem.Id).Erro?.Codigo, Is.EqualTo("Ligacao.Autorreferencia"));
        Assert.That(projeto.ConectarQuadros(origem.Id, destino.Id, 0).FoiFalha, Is.True);
        Assert.That(projeto.ConectarQuadros(origem.Id, destino.Id, 2).FoiFalha, Is.True);
        Assert.That(projeto.ConectarQuadros(origem.Id, destino.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ConectarQuadros(origem.Id, destino.Id).Erro?.Codigo, Is.EqualTo("Ligacao.Duplicada"));
        Assert.That(projeto.Ligacoes, Has.Count.EqualTo(1));
        Assert.That(projeto.Ligacoes.Single().Ordem, Is.EqualTo(1));
    }

    [Test]
    public void ReordenacaoSubstituiObjetosDeValorEPreservaLigacoesDeOutrasOrigens()
    {
        Projeto projeto = NovoProjeto();
        Quadro origem = projeto.CriarQuadro("Origem").Valor;
        Quadro primeiro = projeto.CriarQuadro("Primeiro").Valor;
        Quadro segundo = projeto.CriarQuadro("Segundo").Valor;
        Quadro outraOrigem = projeto.CriarQuadro("Outra origem").Valor;
        projeto.ConectarQuadros(origem.Id, primeiro.Id);
        projeto.ConectarQuadros(origem.Id, segundo.Id);
        projeto.ConectarQuadros(outraOrigem.Id, segundo.Id);
        LigacaoQuadros valorAnterior = projeto.Ligacoes.Single(ligacao => ligacao.QuadroDestinoId == primeiro.Id);

        Assert.That(projeto.ReordenarSucessores(origem.Id, [segundo.Id, primeiro.Id]).FoiSucesso, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(projeto.ObterSucessores(origem.Id), Is.EqualTo(new[] { segundo, primeiro }));
            Assert.That(valorAnterior.Ordem, Is.EqualTo(1));
            Assert.That(projeto.Ligacoes.Single(ligacao => ligacao.QuadroDestinoId == primeiro.Id).Ordem, Is.EqualTo(2));
            Assert.That(projeto.ObterSucessores(outraOrigem.Id), Is.EqualTo(new[] { segundo }));
            Assert.That(projeto.Ligacoes.Single(ligacao => ligacao.QuadroOrigemId == outraOrigem.Id).Ordem, Is.EqualTo(1));
        });
    }

    [Test]
    public void ReordenacaoIncompletaRepetidaOuComQuadroExternoNaoAlteraPrioridades()
    {
        Projeto projeto = NovoProjeto();
        Quadro origem = projeto.CriarQuadro("Origem").Valor;
        Quadro primeiro = projeto.CriarQuadro("Primeiro").Valor;
        Quadro segundo = projeto.CriarQuadro("Segundo").Valor;
        projeto.ConectarQuadros(origem.Id, primeiro.Id);
        projeto.ConectarQuadros(origem.Id, segundo.Id);
        LigacaoQuadros[] anteriores = projeto.Ligacoes.ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(projeto.ReordenarSucessores(origem.Id, [segundo.Id]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarSucessores(origem.Id, [segundo.Id, segundo.Id]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarSucessores(origem.Id, [segundo.Id, Guid.NewGuid()]).FoiFalha, Is.True);
            Assert.That(projeto.Ligacoes, Is.EqualTo(anteriores));
        });
    }

    [Test]
    public void DesconexaoCompactaPrioridadesELiberaRemocaoDoQuadro()
    {
        Projeto projeto = NovoProjeto();
        Quadro origem = projeto.CriarQuadro("Origem").Valor;
        Quadro primeiro = projeto.CriarQuadro("Primeiro").Valor;
        Quadro segundo = projeto.CriarQuadro("Segundo").Valor;
        projeto.ConectarQuadros(origem.Id, primeiro.Id);
        projeto.ConectarQuadros(origem.Id, segundo.Id);
        Assert.That(projeto.RemoverQuadro(primeiro.Id).FoiFalha, Is.True);
        Assert.That(projeto.RemoverQuadro(origem.Id).FoiFalha, Is.True);
        Assert.That(projeto.DesconectarQuadros(origem.Id, primeiro.Id).FoiSucesso, Is.True);
        Assert.That(projeto.Ligacoes.Single().Ordem, Is.EqualTo(1));
        Assert.That(projeto.ObterAntecessores(primeiro.Id), Is.Empty);
        Assert.That(projeto.RemoverQuadro(primeiro.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ObterQuadro(primeiro.Id).FoiFalha, Is.True);
        Assert.That(projeto.ObterQuadro(segundo.Id).Valor, Is.SameAs(segundo));
    }

    [Test]
    public void OrdenacaoIncluiComponentesDesconectadosEQuadrosIsolados()
    {
        Projeto projeto = NovoProjeto();
        Quadro fim = projeto.CriarQuadro("Fim criado antes da origem").Valor;
        Quadro inicio = projeto.CriarQuadro("Início").Valor;
        Quadro isolado = projeto.CriarQuadro("Isolado").Valor;
        Quadro outraOrigem = projeto.CriarQuadro("Outra origem").Valor;
        Quadro outroFim = projeto.CriarQuadro("Outro fim").Valor;
        projeto.ConectarQuadros(inicio.Id, fim.Id);
        projeto.ConectarQuadros(outraOrigem.Id, outroFim.Id);
        Assert.Multiple(() =>
        {
            Assert.That(projeto.ObterQuadrosIniciais(), Is.EquivalentTo(new[] { inicio, isolado, outraOrigem }));
            Assert.That(projeto.ObterQuadrosFinais(), Is.EquivalentTo(new[] { fim, isolado, outroFim }));
            Assert.That(projeto.ExisteCaminho(inicio.Id, outroFim.Id), Is.False);
            Assert.That(projeto.ExisteCaminho(isolado.Id, isolado.Id), Is.True);
            Assert.That(projeto.ExisteCaminho(Guid.NewGuid(), inicio.Id), Is.False);
            Assert.That(projeto.ObterSucessores(Guid.NewGuid()), Is.Empty);
        });
        VerificarOrdemTopologica(projeto);
    }

    [Test]
    public void ProjetoVazioPermiteConsultasEReordenacoesVazias()
    {
        Projeto projeto = NovoProjeto();
        Assert.Multiple(() =>
        {
            Assert.That(projeto.ObterOrdenacaoTopologica(), Is.Empty);
            Assert.That(projeto.ObterQuadrosIniciais(), Is.Empty);
            Assert.That(projeto.ObterQuadrosFinais(), Is.Empty);
            Assert.That(projeto.ReordenarBlocos([]).FoiSucesso, Is.True);
            Assert.That(projeto.ReordenarPontosChave([]).FoiSucesso, Is.True);
            Assert.That(projeto.ConectarQuadros(Guid.NewGuid(), Guid.NewGuid()).FoiFalha, Is.True);
            Assert.That(projeto.DesconectarQuadros(Guid.NewGuid(), Guid.NewGuid()).FoiFalha, Is.True);
        });
    }

    private static Projeto NovoProjeto() => Projeto.Criar("Obra", TipoProjeto.Livro).Valor;

    private static void VerificarOrdemTopologica(Projeto projeto)
    {
        IReadOnlyList<Quadro> ordem = projeto.ObterOrdenacaoTopologica();
        Dictionary<Guid, int> posicoes = ordem.Select((quadro, indice) => (quadro.Id, indice))
            .ToDictionary(item => item.Id, item => item.indice);
        Assert.That(ordem, Is.EquivalentTo(projeto.Quadros));
        foreach (LigacaoQuadros ligacao in projeto.Ligacoes)
            Assert.That(posicoes[ligacao.QuadroOrigemId], Is.LessThan(posicoes[ligacao.QuadroDestinoId]));
    }
}
