using EscribaApp.Dominio.Entidades;
using EscribaApp.Dominio.Enumeracoes;
using EscribaApp.Dominio.ObjetosValor;
using EscribaApp.Dominio.Resultados;

namespace EscribaApp.Dominio.Agregados;

public sealed class Projeto
{
    private readonly List<Genero> _generos = [];
    private readonly List<Personagem> _personagens = [];
    private readonly List<Quadro> _quadros = [];
    private readonly List<Bloco> _blocos = [];
    private readonly List<PontoChave> _pontosChave = [];
    private readonly List<LigacaoQuadros> _ligacoes = [];

    private Projeto(string nome, TipoProjeto tipo, string descricao)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Tipo = tipo;
        Descricao = descricao;
    }

    public Guid Id { get; }
    public string Nome { get; private set; }
    public TipoProjeto Tipo { get; private set; }
    public string Descricao { get; private set; }
    public IReadOnlyCollection<Genero> Generos => _generos.AsReadOnly();
    public IReadOnlyCollection<Personagem> Personagens => _personagens.AsReadOnly();
    public IReadOnlyCollection<Quadro> Quadros => _quadros.AsReadOnly();
    public IReadOnlyList<Bloco> Blocos => _blocos.OrderBy(bloco => bloco.Sequencia).ToList().AsReadOnly();
    public IReadOnlyList<PontoChave> PontosChave => _pontosChave.OrderBy(ponto => ponto.Sequencia).ToList().AsReadOnly();
    public IReadOnlyCollection<LigacaoQuadros> Ligacoes => _ligacoes.AsReadOnly();

    public static Resultado<Projeto> Criar(string nome, TipoProjeto tipo, string descricao = "")
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Resultado<Projeto>.Falha(new Erro("Projeto.NomeObrigatorio", "Informe o nome do projeto."));
        if (!Enum.IsDefined(tipo))
            return Resultado<Projeto>.Falha(new Erro("Projeto.TipoInvalido", "Informe um tipo de projeto válido."));

        return Resultado<Projeto>.Sucesso(new Projeto(nome.Trim(), tipo, Normalizar(descricao)));
    }

    public Resultado AlterarDados(string nome, TipoProjeto tipo, string descricao)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Falha("Projeto.NomeObrigatorio", "Informe o nome do projeto.");
        if (!Enum.IsDefined(tipo))
            return Falha("Projeto.TipoInvalido", "Informe um tipo de projeto válido.");

        Nome = nome.Trim();
        Tipo = tipo;
        Descricao = Normalizar(descricao);
        return Resultado.Sucesso();
    }

    public Resultado AdicionarGenero(string nome)
    {
        Resultado<Genero> resultado = Genero.Criar(nome);
        if (resultado.Erro is { } erro) return Resultado.Falha(erro);
        if (_generos.Contains(resultado.Valor))
            return Falha("Genero.Duplicado", "O gênero já está no projeto.");

        _generos.Add(resultado.Valor);
        return Resultado.Sucesso();
    }

    public Resultado RemoverGenero(string nome)
    {
        Resultado<Genero> resultado = Genero.Criar(nome);
        if (resultado.Erro is { } erro) return Resultado.Falha(erro);
        if (!_generos.Remove(resultado.Valor))
            return Falha("Genero.NaoEncontrado", "O gênero não está no projeto.");
        return Resultado.Sucesso();
    }

    public Resultado<Personagem> CadastrarPersonagem(string nome, string descricao = "", string biotipo = "",
        string personalidade = "", string principaisFeitos = "")
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Resultado<Personagem>.Falha(new Erro("Personagem.NomeObrigatorio", "Informe o nome do personagem."));

        var personagem = new Personagem(nome.Trim(), Normalizar(descricao), Normalizar(biotipo),
            Normalizar(personalidade), Normalizar(principaisFeitos));
        _personagens.Add(personagem);
        return Resultado<Personagem>.Sucesso(personagem);
    }

    public Resultado AtualizarPersonagem(Guid personagemId, string nome, string descricao, string biotipo,
        string personalidade, string principaisFeitos)
    {
        Personagem? personagem = _personagens.Find(item => item.Id == personagemId);
        if (personagem is null) return NaoEncontrado("Personagem");
        if (string.IsNullOrWhiteSpace(nome))
            return Falha("Personagem.NomeObrigatorio", "Informe o nome do personagem.");

        personagem.AlterarDados(nome.Trim(), Normalizar(descricao), Normalizar(biotipo),
            Normalizar(personalidade), Normalizar(principaisFeitos));
        return Resultado.Sucesso();
    }

    public Resultado RemoverPersonagem(Guid personagemId)
    {
        Personagem? personagem = _personagens.Find(item => item.Id == personagemId);
        if (personagem is null) return NaoEncontrado("Personagem");
        if (_quadros.Any(quadro => quadro.PersonagensIds.Contains(personagemId)) ||
            _pontosChave.Any(ponto => ponto.PersonagemResponsavelId == personagemId))
            return Falha("Personagem.EmUso", "Retire as associações e responsabilidades antes de remover o personagem.");

        _personagens.Remove(personagem);
        return Resultado.Sucesso();
    }

    public Resultado<Quadro> CriarQuadro(string nome, string descricao = "")
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Resultado<Quadro>.Falha(new Erro("Quadro.NomeObrigatorio", "Informe o nome do quadro."));

        var quadro = new Quadro(nome.Trim(), Normalizar(descricao));
        _quadros.Add(quadro);
        return Resultado<Quadro>.Sucesso(quadro);
    }

    public Resultado AtualizarQuadro(Guid quadroId, string nome, string descricao)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (string.IsNullOrWhiteSpace(nome))
            return Falha("Quadro.NomeObrigatorio", "Informe o nome do quadro.");

        quadro.AlterarDados(nome.Trim(), Normalizar(descricao));
        return Resultado.Sucesso();
    }

    public Resultado RemoverQuadro(Guid quadroId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (_ligacoes.Any(ligacao => ligacao.QuadroOrigemId == quadroId || ligacao.QuadroDestinoId == quadroId))
            return Falha("Quadro.ComLigacoes", "Desconecte o quadro antes de removê-lo.");

        _quadros.Remove(quadro);
        return Resultado.Sucesso();
    }

    public Resultado<Bloco> CriarBloco(string descricaoObjetivo)
    {
        if (string.IsNullOrWhiteSpace(descricaoObjetivo))
            return Resultado<Bloco>.Falha(new Erro("Bloco.ObjetivoObrigatorio", "Informe o objetivo do bloco."));

        var bloco = new Bloco(_blocos.Count + 1, descricaoObjetivo.Trim());
        _blocos.Add(bloco);
        return Resultado<Bloco>.Sucesso(bloco);
    }

    public Resultado AtualizarBloco(Guid blocoId, string descricaoObjetivo)
    {
        Bloco? bloco = _blocos.Find(item => item.Id == blocoId);
        if (bloco is null) return NaoEncontrado("Bloco");
        if (string.IsNullOrWhiteSpace(descricaoObjetivo))
            return Falha("Bloco.ObjetivoObrigatorio", "Informe o objetivo do bloco.");

        bloco.AlterarObjetivo(descricaoObjetivo.Trim());
        return Resultado.Sucesso();
    }

    public Resultado ReordenarBlocos(IReadOnlyList<Guid> blocosIds)
    {
        Guid[] ordem = blocosIds?.ToArray() ?? [];
        if (blocosIds is null || !ContemTodosSemRepetir(ordem, _blocos.Select(bloco => bloco.Id)))
            return Falha("Bloco.OrdemInvalida", "Informe todos os blocos do projeto, sem repetições.");

        for (int indice = 0; indice < ordem.Length; indice++)
            _blocos.Single(bloco => bloco.Id == ordem[indice]).AlterarSequencia(indice + 1);
        return Resultado.Sucesso();
    }

    public Resultado RemoverBloco(Guid blocoId)
    {
        Bloco? bloco = _blocos.Find(item => item.Id == blocoId);
        if (bloco is null) return NaoEncontrado("Bloco");
        if (_quadros.Any(quadro => quadro.BlocoId == blocoId))
            return Falha("Bloco.EmUso", "Transfira ou desassocie os quadros antes de remover o bloco.");

        _blocos.Remove(bloco);
        ReordenarBlocos(Blocos.Select(item => item.Id).ToArray());
        return Resultado.Sucesso();
    }

    public Resultado<PontoChave> CadastrarPontoChave(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            return Resultado<PontoChave>.Falha(new Erro("PontoChave.DescricaoObrigatoria", "Informe a descrição do ponto-chave."));

        var ponto = new PontoChave(_pontosChave.Count + 1, descricao.Trim());
        _pontosChave.Add(ponto);
        return Resultado<PontoChave>.Sucesso(ponto);
    }

    public Resultado AtualizarPontoChave(Guid pontoChaveId, string descricao)
    {
        PontoChave? ponto = _pontosChave.Find(item => item.Id == pontoChaveId);
        if (ponto is null) return NaoEncontrado("PontoChave");
        if (string.IsNullOrWhiteSpace(descricao))
            return Falha("PontoChave.DescricaoObrigatoria", "Informe a descrição do ponto-chave.");

        ponto.AlterarDescricao(descricao.Trim());
        return Resultado.Sucesso();
    }

    public Resultado ReordenarPontosChave(IReadOnlyList<Guid> pontosChaveIds)
    {
        Guid[] ordem = pontosChaveIds?.ToArray() ?? [];
        if (pontosChaveIds is null || !ContemTodosSemRepetir(ordem, _pontosChave.Select(ponto => ponto.Id)))
            return Falha("PontoChave.OrdemInvalida", "Informe todos os pontos-chave do projeto, sem repetições.");

        for (int indice = 0; indice < ordem.Length; indice++)
            _pontosChave.Single(ponto => ponto.Id == ordem[indice]).AlterarSequencia(indice + 1);
        return Resultado.Sucesso();
    }

    public Resultado RemoverPontoChave(Guid pontoChaveId)
    {
        PontoChave? ponto = _pontosChave.Find(item => item.Id == pontoChaveId);
        if (ponto is null) return NaoEncontrado("PontoChave");
        if (_quadros.Any(quadro => quadro.PontosChaveIds.Contains(pontoChaveId)))
            return Falha("PontoChave.EmUso", "Retire as associações antes de remover o ponto-chave.");

        _pontosChave.Remove(ponto);
        ReordenarPontosChave(PontosChave.Select(item => item.Id).ToArray());
        return Resultado.Sucesso();
    }

    public Resultado DefinirResponsavelPeloPontoChave(Guid pontoChaveId, Guid? personagemId)
    {
        PontoChave? ponto = _pontosChave.Find(item => item.Id == pontoChaveId);
        if (ponto is null) return NaoEncontrado("PontoChave");
        if (personagemId.HasValue && !_personagens.Any(personagem => personagem.Id == personagemId.Value))
            return NaoEncontrado("Personagem");

        ponto.DefinirResponsavel(personagemId);
        return Resultado.Sucesso();
    }

    public Resultado AssociarPersonagemAoQuadro(Guid quadroId, Guid personagemId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (!_personagens.Any(personagem => personagem.Id == personagemId)) return NaoEncontrado("Personagem");
        if (quadro.PersonagensIds.Contains(personagemId))
            return Falha("Personagem.AssociacaoDuplicada", "O personagem já está associado ao quadro.");

        quadro.AssociarPersonagem(personagemId);
        return Resultado.Sucesso();
    }

    public Resultado DesassociarPersonagemDoQuadro(Guid quadroId, Guid personagemId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (!quadro.PersonagensIds.Contains(personagemId))
            return Falha("Personagem.AssociacaoNaoEncontrada", "O personagem não está associado ao quadro.");

        quadro.DesassociarPersonagem(personagemId);
        return Resultado.Sucesso();
    }

    public Resultado AssociarPontoChaveAoQuadro(Guid quadroId, Guid pontoChaveId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (!_pontosChave.Any(ponto => ponto.Id == pontoChaveId)) return NaoEncontrado("PontoChave");
        if (quadro.PontosChaveIds.Contains(pontoChaveId))
            return Falha("PontoChave.AssociacaoDuplicada", "O ponto-chave já está associado ao quadro.");

        quadro.AssociarPontoChave(pontoChaveId);
        return Resultado.Sucesso();
    }

    public Resultado DesassociarPontoChaveDoQuadro(Guid quadroId, Guid pontoChaveId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (!quadro.PontosChaveIds.Contains(pontoChaveId))
            return Falha("PontoChave.AssociacaoNaoEncontrada", "O ponto-chave não está associado ao quadro.");

        quadro.DesassociarPontoChave(pontoChaveId);
        return Resultado.Sucesso();
    }

    public Resultado AssociarQuadroAoBloco(Guid quadroId, Guid blocoId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (!_blocos.Any(bloco => bloco.Id == blocoId)) return NaoEncontrado("Bloco");
        if (quadro.BlocoId == blocoId)
            return Falha("Bloco.AssociacaoDuplicada", "O quadro já pertence ao bloco.");

        quadro.DefinirBloco(blocoId);
        return Resultado.Sucesso();
    }

    public Resultado DesassociarQuadroDoBloco(Guid quadroId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        if (quadro.BlocoId is null)
            return Falha("Bloco.AssociacaoNaoEncontrada", "O quadro não pertence a um bloco.");

        quadro.DefinirBloco(null);
        return Resultado.Sucesso();
    }

    public Resultado AdicionarObjetivoAoQuadro(Guid quadroId, string descricao)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        Resultado<ObjetivoNarrativo> resultado = ObjetivoNarrativo.Criar(descricao);
        if (resultado.Erro is { } erro) return Resultado.Falha(erro);
        if (quadro.ObjetivosNarrativos.Contains(resultado.Valor))
            return Falha("Objetivo.Duplicado", "O objetivo já está no quadro.");

        quadro.AdicionarObjetivo(resultado.Valor);
        return Resultado.Sucesso();
    }

    public Resultado RemoverObjetivoDoQuadro(Guid quadroId, string descricao)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        if (quadro is null) return NaoEncontrado("Quadro");
        Resultado<ObjetivoNarrativo> resultado = ObjetivoNarrativo.Criar(descricao);
        if (resultado.Erro is { } erro) return Resultado.Falha(erro);
        if (!quadro.ObjetivosNarrativos.Contains(resultado.Valor))
            return NaoEncontrado("Objetivo");

        quadro.RemoverObjetivo(resultado.Valor);
        return Resultado.Sucesso();
    }

    public Resultado ConectarQuadros(Guid quadroOrigemId, Guid quadroDestinoId, int? ordem = null)
    {
        if (!_quadros.Any(quadro => quadro.Id == quadroOrigemId) ||
            !_quadros.Any(quadro => quadro.Id == quadroDestinoId)) return NaoEncontrado("Quadro");

        int quantidadeSaidas = _ligacoes.Count(ligacao => ligacao.QuadroOrigemId == quadroOrigemId);
        int posicao = ordem ?? quantidadeSaidas + 1;
        Resultado<LigacaoQuadros> resultado = LigacaoQuadros.Criar(quadroOrigemId, quadroDestinoId, posicao);
        if (resultado.Erro is { } erro) return Resultado.Falha(erro);
        if (_ligacoes.Any(ligacao => ligacao.QuadroOrigemId == quadroOrigemId && ligacao.QuadroDestinoId == quadroDestinoId))
            return Falha("Ligacao.Duplicada", "A ligação já existe.");
        if (posicao != quantidadeSaidas + 1)
            return Falha("Ligacao.OrdemInvalida", "Adicione a ligação ao final e use ReordenarSucessores para mudar a prioridade.");
        if (ExisteCaminho(quadroDestinoId, quadroOrigemId))
            return Falha("Ligacao.Ciclo", "A ligação criaria um ciclo na escaleta.");

        _ligacoes.Add(resultado.Valor);
        return Resultado.Sucesso();
    }

    public Resultado DesconectarQuadros(Guid quadroOrigemId, Guid quadroDestinoId)
    {
        LigacaoQuadros? ligacao = _ligacoes.Find(item => item.QuadroOrigemId == quadroOrigemId && item.QuadroDestinoId == quadroDestinoId);
        if (ligacao is null) return NaoEncontrado("Ligacao");

        _ligacoes.Remove(ligacao);
        Guid[] ordem = ObterSucessores(quadroOrigemId).Select(quadro => quadro.Id).ToArray();
        ReordenarSucessores(quadroOrigemId, ordem);
        return Resultado.Sucesso();
    }

    public Resultado ReordenarSucessores(Guid quadroOrigemId, IReadOnlyList<Guid> sucessoresIds)
    {
        if (!_quadros.Any(quadro => quadro.Id == quadroOrigemId)) return NaoEncontrado("Quadro");
        Guid[] ordem = sucessoresIds?.ToArray() ?? [];
        IEnumerable<Guid> atuais = _ligacoes.Where(ligacao => ligacao.QuadroOrigemId == quadroOrigemId)
            .Select(ligacao => ligacao.QuadroDestinoId);
        if (sucessoresIds is null || !ContemTodosSemRepetir(ordem, atuais))
            return Falha("Ligacao.OrdemInvalida", "Informe todos os sucessores diretos, sem repetições.");

        // Objetos de valor são substituídos, nunca alterados.
        var novasLigacoes = new List<LigacaoQuadros>();
        for (int indice = 0; indice < ordem.Length; indice++)
            novasLigacoes.Add(LigacaoQuadros.Criar(quadroOrigemId, ordem[indice], indice + 1).Valor);

        _ligacoes.RemoveAll(ligacao => ligacao.QuadroOrigemId == quadroOrigemId);
        _ligacoes.AddRange(novasLigacoes);
        return Resultado.Sucesso();
    }

    public Resultado<Quadro> ObterQuadro(Guid quadroId)
    {
        Quadro? quadro = _quadros.Find(item => item.Id == quadroId);
        return quadro is null
            ? Resultado<Quadro>.Falha(new Erro("Quadro.NaoEncontrado", "Quadro não encontrado neste projeto."))
            : Resultado<Quadro>.Sucesso(quadro);
    }

    public IReadOnlyList<Quadro> ObterSucessores(Guid quadroId)
    {
        Dictionary<Guid, Quadro> quadros = _quadros.ToDictionary(quadro => quadro.Id);
        return _ligacoes.Where(ligacao => ligacao.QuadroOrigemId == quadroId)
            .OrderBy(ligacao => ligacao.Ordem)
            .Select(ligacao => quadros[ligacao.QuadroDestinoId]).ToList().AsReadOnly();
    }

    public IReadOnlyCollection<Quadro> ObterAntecessores(Guid quadroId)
    {
        HashSet<Guid> origens = _ligacoes.Where(ligacao => ligacao.QuadroDestinoId == quadroId)
            .Select(ligacao => ligacao.QuadroOrigemId).ToHashSet();
        return _quadros.Where(quadro => origens.Contains(quadro.Id)).ToList().AsReadOnly();
    }

    public IReadOnlyCollection<Quadro> ObterQuadrosIniciais()
    {
        HashSet<Guid> destinos = _ligacoes.Select(ligacao => ligacao.QuadroDestinoId).ToHashSet();
        return _quadros.Where(quadro => !destinos.Contains(quadro.Id)).ToList().AsReadOnly();
    }

    public IReadOnlyCollection<Quadro> ObterQuadrosFinais()
    {
        HashSet<Guid> origens = _ligacoes.Select(ligacao => ligacao.QuadroOrigemId).ToHashSet();
        return _quadros.Where(quadro => !origens.Contains(quadro.Id)).ToList().AsReadOnly();
    }

    public IReadOnlyCollection<Quadro> ObterQuadrosDoBloco(Guid blocoId) =>
        _quadros.Where(quadro => quadro.BlocoId == blocoId).ToList().AsReadOnly();

    public IReadOnlyCollection<Quadro> ObterQuadrosDoPersonagem(Guid personagemId) =>
        _quadros.Where(quadro => quadro.PersonagensIds.Contains(personagemId)).ToList().AsReadOnly();

    public IReadOnlyCollection<Quadro> ObterQuadrosDoPontoChave(Guid pontoChaveId) =>
        _quadros.Where(quadro => quadro.PontosChaveIds.Contains(pontoChaveId)).ToList().AsReadOnly();

    public bool ExisteCaminho(Guid quadroOrigemId, Guid quadroDestinoId)
    {
        if (!_quadros.Any(quadro => quadro.Id == quadroOrigemId) ||
            !_quadros.Any(quadro => quadro.Id == quadroDestinoId)) return false;

        ILookup<Guid, Guid> saidas = _ligacoes.ToLookup(ligacao => ligacao.QuadroOrigemId, ligacao => ligacao.QuadroDestinoId);
        var visitados = new HashSet<Guid>();
        var pendentes = new Stack<Guid>();
        pendentes.Push(quadroOrigemId);
        while (pendentes.TryPop(out Guid atual))
        {
            if (!visitados.Add(atual)) continue;
            if (atual == quadroDestinoId) return true;
            foreach (Guid sucessorId in saidas[atual]) pendentes.Push(sucessorId);
        }
        return false;
    }

    public IReadOnlyList<Quadro> ObterOrdenacaoTopologica()
    {
        Dictionary<Guid, Quadro> quadros = _quadros.ToDictionary(quadro => quadro.Id);
        Dictionary<Guid, int> entradas = _quadros.ToDictionary(quadro => quadro.Id, _ => 0);
        foreach (LigacaoQuadros ligacao in _ligacoes) entradas[ligacao.QuadroDestinoId]++;

        ILookup<Guid, LigacaoQuadros> saidas = _ligacoes.OrderBy(ligacao => ligacao.Ordem)
            .ToLookup(ligacao => ligacao.QuadroOrigemId);
        var pendentes = new Queue<Guid>(_quadros.Where(quadro => entradas[quadro.Id] == 0).Select(quadro => quadro.Id));
        var ordenados = new List<Quadro>();
        while (pendentes.TryDequeue(out Guid atual))
        {
            ordenados.Add(quadros[atual]);
            foreach (LigacaoQuadros ligacao in saidas[atual])
            {
                if (--entradas[ligacao.QuadroDestinoId] == 0)
                    pendentes.Enqueue(ligacao.QuadroDestinoId);
            }
        }

        if (ordenados.Count != _quadros.Count)
            throw new InvalidOperationException("A estrutura interna da escaleta contém um ciclo.");
        return ordenados.AsReadOnly();
    }

    private static string Normalizar(string? texto) => texto?.Trim() ?? string.Empty;

    private static bool ContemTodosSemRepetir(Guid[] ordem, IEnumerable<Guid> atuais) =>
        ordem.Distinct().Count() == ordem.Length && ordem.ToHashSet().SetEquals(atuais);

    private static Resultado Falha(string codigo, string mensagem) => Resultado.Falha(new Erro(codigo, mensagem));

    private static Resultado NaoEncontrado(string conceito) =>
        Falha($"{conceito}.NaoEncontrado", $"{conceito} não encontrado neste projeto.");
}
