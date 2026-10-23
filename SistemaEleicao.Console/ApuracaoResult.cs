namespace SistemaEleicao.Console;

/// <summary>
/// Resultado da apuração da eleição: classificação dos candidatos,
/// votos brancos/nulos e candidato(s) vencedor(es).
/// </summary>
/// <param name="Classificacao">Candidatos ordenados por votos (decrescente).</param>
/// <param name="VotosBrancos">Total de votos em branco.</param>
/// <param name="VotosNulos">Total de votos nulos.</param>
/// <param name="Vencedores">Candidato(s) com mais votos; vazio se não houve votos.</param>
public record ApuracaoResult(
    IReadOnlyList<Candidato> Classificacao,
    int VotosBrancos,
    int VotosNulos,
    IReadOnlyList<Candidato> Vencedores);
