namespace SistemaEleicao.Core;

/// <summary>
/// Classe de gerência da eleição: cadastro de candidatos, controle de
/// eleitores, registro de votos e apuração do resultado.
/// </summary>
public class EleicaoManager
{
    /// <summary>Número que representa o voto em branco na urna.</summary>
    public const int NumeroVotoBranco = 0;

    private readonly List<Candidato> _candidatos = new();
    private readonly HashSet<string> _eleitoresRegistrados = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _eleitoresQueVotaram = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Lista somente leitura dos candidatos cadastrados.</summary>
    public IReadOnlyList<Candidato> Candidatos => _candidatos.AsReadOnly();

    /// <summary>Total de eleitores registrados para a votação.</summary>
    public int TotalEleitoresRegistrados => _eleitoresRegistrados.Count;

    /// <summary>Total de votos em branco contabilizados.</summary>
    public int VotosBrancos { get; private set; }

    /// <summary>Total de votos nulos contabilizados.</summary>
    public int VotosNulos { get; private set; }

    /// <summary>
    /// Cadastra um candidato na eleição.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando já existe um candidato com o mesmo número.
    /// </exception>
    public Candidato CadastrarCandidato(string nome, int numero)
    {
        if (_candidatos.Any(c => c.Numero == numero))
            throw new InvalidOperationException(
                $"Já existe um candidato cadastrado com o número {numero}.");

        var candidato = new Candidato(nome, numero);
        _candidatos.Add(candidato);
        return candidato;
    }

    /// <summary>
    /// Registra um eleitor pelo número do título/CPF (simulado).
    /// </summary>
    /// <returns><c>true</c> se o eleitor foi registrado; <c>false</c> se já existia ou é inválido.</returns>
    public bool RegistrarEleitor(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return false;

        return _eleitoresRegistrados.Add(titulo.Trim());
    }

    /// <summary>
    /// Registra o voto de um eleitor. O eleitor precisa estar registrado
    /// e não pode votar duas vezes. Voto em número inexistente é nulo;
    /// o número 0 representa voto em branco.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando o eleitor não está registrado ou já votou.
    /// </exception>
    public TipoVoto Votar(string titulo, int numeroCandidato)
    {
        var documento = titulo?.Trim() ?? string.Empty;

        if (!_eleitoresRegistrados.Contains(documento))
            throw new InvalidOperationException(
                "Eleitor não registrado. Registre o título/CPF antes de votar.");

        if (!_eleitoresQueVotaram.Add(documento))
            throw new InvalidOperationException("Este eleitor já votou nesta eleição.");

        if (numeroCandidato == NumeroVotoBranco)
        {
            VotosBrancos++;
            return TipoVoto.Branco;
        }

        var candidato = _candidatos.FirstOrDefault(c => c.Numero == numeroCandidato);
        if (candidato is null)
        {
            VotosNulos++;
            return TipoVoto.Nulo;
        }

        candidato.RegistrarVoto();
        return TipoVoto.Candidato;
    }

    /// <summary>
    /// Apura a eleição: classifica os candidatos por votos e declara
    /// o(s) vencedor(es). Empate retorna todos os empatados; sem votos,
    /// nenhum vencedor é declarado.
    /// </summary>
    public ApuracaoResult Apurar()
    {
        var classificacao = _candidatos
            .OrderByDescending(c => c.Votos)
            .ThenBy(c => c.Numero)
            .ToList();

        var maxVotos = classificacao.Count == 0 ? 0 : classificacao[0].Votos;
        var vencedores = maxVotos == 0
            ? new List<Candidato>()
            : classificacao.Where(c => c.Votos == maxVotos).ToList();

        return new ApuracaoResult(classificacao.AsReadOnly(), VotosBrancos, VotosNulos, vencedores.AsReadOnly());
    }
}
