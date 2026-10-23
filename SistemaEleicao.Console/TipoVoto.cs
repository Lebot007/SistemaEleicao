namespace SistemaEleicao.Console;

/// <summary>
/// Tipo de voto registrado na urna.
/// </summary>
public enum TipoVoto
{
    /// <summary>Voto válido em um candidato cadastrado.</summary>
    Candidato,

    /// <summary>Voto em branco (número 0).</summary>
    Branco,

    /// <summary>Voto nulo (número de candidato inexistente).</summary>
    Nulo
}
