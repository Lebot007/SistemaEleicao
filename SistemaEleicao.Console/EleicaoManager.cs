namespace SistemaEleicao.Console;

/// <summary>
/// Classe de gerência da eleição (Etapa 1): cadastro de candidatos
/// com validação para evitar números duplicados.
/// </summary>
public class EleicaoManager
{
    private readonly List<Candidato> _candidatos = new();

    /// <summary>Lista somente leitura dos candidatos cadastrados.</summary>
    public IReadOnlyList<Candidato> Candidatos => _candidatos.AsReadOnly();

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
}
