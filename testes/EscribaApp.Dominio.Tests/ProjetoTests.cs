using EscribaApp.Dominio.Agregados;
using EscribaApp.Dominio.Entidades;
using EscribaApp.Dominio.Enumeracoes;
using EscribaApp.Dominio.ObjetosValor;
using NUnit.Framework;

namespace EscribaApp.Dominio.Tests;

[TestFixture]
public class ProjetoTests
{
    [Test]
    public void CriacaoValidaNomeETipoEPermiteRascunhos()
    {
        Projeto projeto = NovoProjeto();
        Assert.Multiple(() =>
        {
            Assert.That(Projeto.Criar(" ", TipoProjeto.Livro).FoiFalha, Is.True);
            Assert.That(Projeto.Criar("Obra", (TipoProjeto)999).FoiFalha, Is.True);
            Assert.That(projeto.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(projeto.CriarQuadro("Cena").Valor.Descricao, Is.Empty);
            Assert.That(projeto.CadastrarPersonagem("Ana").Valor.Personalidade, Is.Empty);
            Assert.That(projeto.CriarBloco(" ").FoiFalha, Is.True);
            Assert.That(projeto.CadastrarPontoChave(" ").FoiFalha, Is.True);
        });
    }

    [Test]
    public void AlteracoesInvalidasPreservamDadosAnteriores()
    {
        Projeto projeto = NovoProjeto();
        Personagem personagem = projeto.CadastrarPersonagem("Ana", "Descrição", "Biotipo", "Personalidade", "Feitos").Valor;
        Quadro quadro = projeto.CriarQuadro("Cena", "Descrição").Valor;
        Bloco bloco = projeto.CriarBloco("Apresentação").Valor;
        PontoChave ponto = projeto.CadastrarPontoChave("Revelação").Valor;

        Assert.Multiple(() =>
        {
            Assert.That(projeto.AlterarDados("Alterado", (TipoProjeto)999, "Nova").FoiFalha, Is.True);
            Assert.That(projeto.AtualizarPersonagem(personagem.Id, " ", "Nova", "", "", "").FoiFalha, Is.True);
            Assert.That(projeto.AtualizarQuadro(quadro.Id, " ", "Nova").FoiFalha, Is.True);
            Assert.That(projeto.AtualizarBloco(bloco.Id, " ").FoiFalha, Is.True);
            Assert.That(projeto.AtualizarPontoChave(ponto.Id, " ").FoiFalha, Is.True);
            Assert.That(projeto.Nome, Is.EqualTo("Obra"));
            Assert.That(personagem.Descricao, Is.EqualTo("Descrição"));
            Assert.That(quadro.Descricao, Is.EqualTo("Descrição"));
            Assert.That(bloco.DescricaoObjetivo, Is.EqualTo("Apresentação"));
            Assert.That(ponto.Descricao, Is.EqualTo("Revelação"));
        });
    }

    [Test]
    public void AtualizacoesValidasNormalizamTextosEPreservamIdentidades()
    {
        Projeto projeto = NovoProjeto();
        Personagem personagem = projeto.CadastrarPersonagem("Ana").Valor;
        Quadro quadro = projeto.CriarQuadro("Cena").Valor;
        Bloco bloco = projeto.CriarBloco("Início").Valor;
        PontoChave ponto = projeto.CadastrarPontoChave("Pista").Valor;
        Guid personagemId = personagem.Id;

        Assert.Multiple(() =>
        {
            Assert.That(projeto.AlterarDados(" Nova obra ", TipoProjeto.Filme, " Sinopse ").FoiSucesso, Is.True);
            Assert.That(projeto.AtualizarPersonagem(personagem.Id, " Bia ", " Autora ", " Alta ", " Curiosa ", " Investiga ").FoiSucesso, Is.True);
            Assert.That(projeto.AtualizarQuadro(quadro.Id, " Abertura ", " Encontro ").FoiSucesso, Is.True);
            Assert.That(projeto.AtualizarBloco(bloco.Id, " Apresentar ").FoiSucesso, Is.True);
            Assert.That(projeto.AtualizarPontoChave(ponto.Id, " Chave ").FoiSucesso, Is.True);
            Assert.That(projeto.Nome, Is.EqualTo("Nova obra"));
            Assert.That(projeto.Tipo, Is.EqualTo(TipoProjeto.Filme));
            Assert.That(personagem.Id, Is.EqualTo(personagemId));
            Assert.That(personagem.Personalidade, Is.EqualTo("Curiosa"));
            Assert.That(personagem.PrincipaisFeitos, Is.EqualTo("Investiga"));
            Assert.That(quadro.Nome, Is.EqualTo("Abertura"));
            Assert.That(bloco.DescricaoObjetivo, Is.EqualTo("Apresentar"));
            Assert.That(ponto.Descricao, Is.EqualTo("Chave"));
        });
    }

    [Test]
    public void GenerosNaoSeDuplicamEAceitamRemocaoNormalizada()
    {
        Projeto projeto = NovoProjeto();
        Assert.Multiple(() =>
        {
            Assert.That(projeto.AdicionarGenero(" Drama ").FoiSucesso, Is.True);
            Assert.That(projeto.AdicionarGenero("drama").FoiFalha, Is.True);
            Assert.That(projeto.AdicionarGenero("Aventura").FoiSucesso, Is.True);
            Assert.That(projeto.Generos, Has.Count.EqualTo(2));
            Assert.That(projeto.RemoverGenero("DRAMA").FoiSucesso, Is.True);
            Assert.That(projeto.RemoverGenero("Drama").FoiFalha, Is.True);
        });
    }

    [Test]
    public void ElementosDeOutroProjetoNaoPodemSerAssociados()
    {
        Projeto projeto = NovoProjeto();
        Projeto outro = NovoProjeto();
        Quadro quadro = projeto.CriarQuadro("Cena").Valor;
        Quadro quadroExterno = outro.CriarQuadro("Cena externa").Valor;
        Personagem personagemExterno = outro.CadastrarPersonagem("Ana").Valor;
        PontoChave pontoExterno = outro.CadastrarPontoChave("Pista").Valor;
        Bloco blocoExterno = outro.CriarBloco("Abertura").Valor;
        PontoChave ponto = projeto.CadastrarPontoChave("Segredo").Valor;
        Assert.Multiple(() =>
        {
            Assert.That(projeto.AssociarPersonagemAoQuadro(quadro.Id, personagemExterno.Id).FoiFalha, Is.True);
            Assert.That(projeto.AssociarPontoChaveAoQuadro(quadro.Id, pontoExterno.Id).FoiFalha, Is.True);
            Assert.That(projeto.AssociarQuadroAoBloco(quadro.Id, blocoExterno.Id).FoiFalha, Is.True);
            Assert.That(projeto.DefinirResponsavelPeloPontoChave(ponto.Id, personagemExterno.Id).FoiFalha, Is.True);
            Assert.That(projeto.ConectarQuadros(quadro.Id, quadroExterno.Id).FoiFalha, Is.True);
            Assert.That(quadro.PersonagensIds, Is.Empty);
            Assert.That(quadro.PontosChaveIds, Is.Empty);
            Assert.That(quadro.BlocoId, Is.Null);
            Assert.That(ponto.PersonagemResponsavelId, Is.Null);
            Assert.That(projeto.Ligacoes, Is.Empty);
        });
    }

    [Test]
    public void PersonagemEmUsoSoPodeSerRemovidoDepoisDeRetirarTodasAsReferencias()
    {
        Projeto projeto = NovoProjeto();
        Quadro quadro = projeto.CriarQuadro("Cena").Valor;
        Personagem personagem = projeto.CadastrarPersonagem("Ana").Valor;
        PontoChave ponto = projeto.CadastrarPontoChave("Segredo").Valor;
        Assert.That(projeto.AssociarPersonagemAoQuadro(quadro.Id, personagem.Id).FoiSucesso, Is.True);
        Assert.That(projeto.AssociarPersonagemAoQuadro(quadro.Id, personagem.Id).FoiFalha, Is.True);
        Assert.That(projeto.DefinirResponsavelPeloPontoChave(ponto.Id, personagem.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ObterQuadrosDoPersonagem(personagem.Id), Is.EqualTo(new[] { quadro }));
        Assert.That(projeto.RemoverPersonagem(personagem.Id).FoiFalha, Is.True);
        Assert.That(projeto.DesassociarPersonagemDoQuadro(quadro.Id, personagem.Id).FoiSucesso, Is.True);
        Assert.That(projeto.RemoverPersonagem(personagem.Id).FoiFalha, Is.True);
        Assert.That(projeto.DefinirResponsavelPeloPontoChave(ponto.Id, null).FoiSucesso, Is.True);
        Assert.That(projeto.RemoverPersonagem(personagem.Id).FoiSucesso, Is.True);
        Assert.That(projeto.Personagens, Is.Empty);
    }

    [Test]
    public void PontoChavePodeParticiparDeVariasRamificacoesMasNaoSerRemovidoEnquantoUsado()
    {
        Projeto projeto = NovoProjeto();
        Quadro primeiro = projeto.CriarQuadro("Alternativa A").Valor;
        Quadro segundo = projeto.CriarQuadro("Alternativa B").Valor;
        PontoChave ponto = projeto.CadastrarPontoChave("Revelação").Valor;
        Assert.That(projeto.AssociarPontoChaveAoQuadro(primeiro.Id, ponto.Id).FoiSucesso, Is.True);
        Assert.That(projeto.AssociarPontoChaveAoQuadro(segundo.Id, ponto.Id).FoiSucesso, Is.True);
        Assert.That(projeto.AssociarPontoChaveAoQuadro(primeiro.Id, ponto.Id).FoiFalha, Is.True);
        Assert.That(projeto.ObterQuadrosDoPontoChave(ponto.Id), Is.EquivalentTo(new[] { primeiro, segundo }));
        Assert.That(projeto.RemoverPontoChave(ponto.Id).FoiFalha, Is.True);
        projeto.DesassociarPontoChaveDoQuadro(primeiro.Id, ponto.Id);
        Assert.That(projeto.RemoverPontoChave(ponto.Id).FoiFalha, Is.True);
        projeto.DesassociarPontoChaveDoQuadro(segundo.Id, ponto.Id);
        Assert.That(projeto.RemoverPontoChave(ponto.Id).FoiSucesso, Is.True);
    }

    [Test]
    public void QuadroPodeSerTransferidoDeBlocoSemDuplicarAssociacao()
    {
        Projeto projeto = NovoProjeto();
        Quadro quadro = projeto.CriarQuadro("Cena").Valor;
        Bloco primeiro = projeto.CriarBloco("Início").Valor;
        Bloco segundo = projeto.CriarBloco("Desenvolvimento").Valor;
        Assert.That(projeto.AssociarQuadroAoBloco(quadro.Id, primeiro.Id).FoiSucesso, Is.True);
        Assert.That(projeto.AssociarQuadroAoBloco(quadro.Id, primeiro.Id).FoiFalha, Is.True);
        Assert.That(projeto.RemoverBloco(primeiro.Id).FoiFalha, Is.True);
        Assert.That(projeto.AssociarQuadroAoBloco(quadro.Id, segundo.Id).FoiSucesso, Is.True);
        Assert.That(projeto.ObterQuadrosDoBloco(primeiro.Id), Is.Empty);
        Assert.That(projeto.ObterQuadrosDoBloco(segundo.Id), Is.EqualTo(new[] { quadro }));
        Assert.That(projeto.RemoverBloco(primeiro.Id).FoiSucesso, Is.True);
        Assert.That(segundo.Sequencia, Is.EqualTo(1));
        Assert.That(projeto.DesassociarQuadroDoBloco(quadro.Id).FoiSucesso, Is.True);
        Assert.That(projeto.RemoverBloco(segundo.Id).FoiSucesso, Is.True);
    }

    [Test]
    public void ObjetivosSaoValidadosComparadosPorValorERemovidos()
    {
        Projeto projeto = NovoProjeto();
        Quadro quadro = projeto.CriarQuadro("Cena").Valor;
        Assert.Multiple(() =>
        {
            Assert.That(projeto.AdicionarObjetivoAoQuadro(quadro.Id, " ").FoiFalha, Is.True);
            Assert.That(projeto.AdicionarObjetivoAoQuadro(quadro.Id, " Revelar segredo ").FoiSucesso, Is.True);
            Assert.That(projeto.AdicionarObjetivoAoQuadro(quadro.Id, "Revelar segredo").FoiFalha, Is.True);
            Assert.That(projeto.RemoverObjetivoDoQuadro(quadro.Id, "Revelar segredo").FoiSucesso, Is.True);
            Assert.That(quadro.ObjetivosNarrativos, Is.Empty);
        });
    }

    [Test]
    public void ReordenacoesDeBlocosEPontosSaoAtomicasESequenciasSaoCompactadasNaRemocao()
    {
        Projeto projeto = NovoProjeto();
        Bloco primeiro = projeto.CriarBloco("Primeiro").Valor;
        Bloco segundo = projeto.CriarBloco("Segundo").Valor;
        PontoChave pista = projeto.CadastrarPontoChave("Pista").Valor;
        PontoChave revelacao = projeto.CadastrarPontoChave("Revelação").Valor;
        Assert.That(projeto.ReordenarBlocos([segundo.Id, primeiro.Id]).FoiSucesso, Is.True);
        Assert.That(projeto.ReordenarPontosChave([revelacao.Id, pista.Id]).FoiSucesso, Is.True);
        Assert.Multiple(() =>
        {
            Assert.That(projeto.ReordenarBlocos([primeiro.Id, primeiro.Id]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarBlocos([primeiro.Id]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarBlocos([primeiro.Id, Guid.NewGuid()]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarPontosChave([pista.Id, pista.Id]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarPontosChave([pista.Id]).FoiFalha, Is.True);
            Assert.That(projeto.ReordenarPontosChave([pista.Id, Guid.NewGuid()]).FoiFalha, Is.True);
            Assert.That(projeto.Blocos.Select(bloco => bloco.Id), Is.EqualTo(new[] { segundo.Id, primeiro.Id }));
            Assert.That(projeto.PontosChave.Select(ponto => ponto.Id), Is.EqualTo(new[] { revelacao.Id, pista.Id }));
        });
        projeto.RemoverBloco(segundo.Id);
        projeto.RemoverPontoChave(revelacao.Id);
        Assert.That(primeiro.Sequencia, Is.EqualTo(1));
        Assert.That(pista.Sequencia, Is.EqualTo(1));
        Assert.That(projeto.CriarBloco("Novo").Valor.Sequencia, Is.EqualTo(2));
        Assert.That(projeto.CadastrarPontoChave("Nova pista").Valor.Sequencia, Is.EqualTo(2));
    }

    [Test]
    public void ColecoesExpostasNaoPermitemModificarOAgregado()
    {
        Projeto projeto = NovoProjeto();
        Quadro quadro = projeto.CriarQuadro("Cena").Valor;
        projeto.AdicionarObjetivoAoQuadro(quadro.Id, "Apresentar personagem");
        Assert.Multiple(() =>
        {
            Assert.Throws<NotSupportedException>(() => ((ICollection<Quadro>)projeto.Quadros).Clear());
            Assert.Throws<NotSupportedException>(() => ((ICollection<Guid>)quadro.PersonagensIds).Add(Guid.NewGuid()));
            Assert.Throws<NotSupportedException>(() => ((ICollection<Guid>)quadro.PontosChaveIds).Add(Guid.NewGuid()));
            Assert.Throws<NotSupportedException>(() => ((ICollection<ObjetivoNarrativo>)quadro.ObjetivosNarrativos).Clear());
            Assert.Throws<NotSupportedException>(() => ((ICollection<LigacaoQuadros>)projeto.Ligacoes).Clear());
        });
        Assert.That(projeto.Quadros, Has.Count.EqualTo(1));
        Assert.That(quadro.ObjetivosNarrativos, Has.Count.EqualTo(1));
    }

    private static Projeto NovoProjeto() => Projeto.Criar("Obra", TipoProjeto.Livro).Valor;
}
