using System.Text;
using SistemaEleicao.Core;

namespace SistemaEleicao.Ui;

/// <summary>
/// Camada de apresentação da apuração: converte o <see cref="ApuracaoResult"/>
/// em texto. É uma classe pura, sem nenhuma dependência de <see cref="Console"/>,
/// o que permite cobrir a formatação do relatório com testes unitários.
/// </summary>
public static class ApuracaoReport
{
    /// <summary>Cabeçalho exibido acima do relatório final.</summary>
    public const string Cabecalho = "========== APURAÇÃO FINAL ==========";

    /// <summary>Mensagem exibida quando não há nenhum candidato cadastrado.</summary>
    public const string SemCandidatos = "Nenhum candidato cadastrado.";

    /// <summary>Mensagem exibida quando a eleição terminou sem votos computados.</summary>
    public const string SemVencedor = "Eleição sem votos computados: não há vencedor.";

    /// <summary>
    /// Gera, linha a linha, o relatório final da apuração: classificação dos
    /// candidatos, votos brancos, votos nulos e o(s) vencedor(es).
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Lançada quando <paramref name="resultado"/> é <c>null</c>.
    /// </exception>
    public static IReadOnlyList<string> GerarLinhas(ApuracaoResult resultado)
    {
        ArgumentNullException.ThrowIfNull(resultado);

        var linhas = new List<string> { Cabecalho };

        if (resultado.Classificacao.Count == 0)
            linhas.Add(SemCandidatos);

        foreach (var candidato in resultado.Classificacao)
            linhas.Add($"{candidato.Numero} - {candidato.Nome}: {candidato.Votos} voto(s)");

        linhas.Add($"Votos brancos: {resultado.VotosBrancos}");
        linhas.Add($"Votos nulos: {resultado.VotosNulos}");
        linhas.Add(DescreverVencedores(resultado.Vencedores));

        return linhas;
    }

    /// <summary>
    /// Renderiza o relatório completo em uma única <see cref="string"/>,
    /// com as linhas separadas por <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string Renderizar(ApuracaoResult resultado)
    {
        var builder = new StringBuilder();
        foreach (var linha in GerarLinhas(resultado))
            builder.AppendLine(linha);

        return builder.ToString().TrimEnd('\r', '\n');
    }

    private static string DescreverVencedores(IReadOnlyList<Candidato> vencedores) => vencedores.Count switch
    {
        0 => SemVencedor,
        1 => $"VENCEDOR: {Formatar(vencedores[0])} com {vencedores[0].Votos} voto(s).",
        _ => $"EMPATE entre: {string.Join(", ", vencedores.Select(Formatar))}"
    };

    private static string Formatar(Candidato candidato) => $"{candidato.Nome} (número {candidato.Numero})";
}
