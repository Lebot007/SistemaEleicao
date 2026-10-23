using SistemaEleicao.Core;
using SistemaEleicao.Ui;
using Xunit;

namespace SistemaEleicao.Tests;

/// <summary>
/// Etapa 3 - Testes unitários da camada de apresentação da apuração.
/// O relatório final (total de votos por candidato, brancos, nulos e
/// vencedor) é gerado por <see cref="ApuracaoReport"/>, uma classe pura
/// sem dependência de <see cref="Console"/>, o que torna a formatação
/// testável de forma isolada.
/// </summary>
public class ApuracaoReportTests
{
    private static EleicaoManager CriarEleicaoComDoisCandidatos()
    {
        var manager = new EleicaoManager();
        manager.CadastrarCandidato("Maria Silva", 10);
        manager.CadastrarCandidato("João Souza", 20);
        return manager;
    }

    [Fact]
    public void GerarLinhas_SemCandidatos_IndicaQueNenhumCandidatoFoiCadastrado()
    {
        var manager = new EleicaoManager();

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());

        Assert.Contains("Nenhum candidato cadastrado.", linhas);
    }

    [Fact]
    public void GerarLinhas_ComCandidatos_ListaNumeroNomeEVotos()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.Votar("111", 10);

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());

        Assert.Contains("10 - Maria Silva: 1 voto(s)", linhas);
        Assert.Contains("20 - João Souza: 0 voto(s)", linhas);
    }

    [Fact]
    public void GerarLinhas_OrdenaCandidatosPorQuantidadeDeVotosDecrescente()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.RegistrarEleitor("333");
        manager.Votar("111", 20);
        manager.Votar("222", 20);
        manager.Votar("333", 10);

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());
        var posicaoJoao = linhas.ToList().FindIndex(l => l.Contains("João Souza"));
        var posicaoMaria = linhas.ToList().FindIndex(l => l.Contains("Maria Silva"));

        Assert.True(posicaoJoao < posicaoMaria, "O candidato com mais votos deve aparecer primeiro.");
    }

    [Fact]
    public void GerarLinhas_ExibeTotaisDeVotosBrancosENulos()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.Votar("111", EleicaoManager.NumeroVotoBranco);
        manager.Votar("222", 99);

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());

        Assert.Contains("Votos brancos: 1", linhas);
        Assert.Contains("Votos nulos: 1", linhas);
    }

    [Fact]
    public void GerarLinhas_ComVencedorUnico_ExibeLinhaDeVencedor()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.Votar("111", 10);
        manager.Votar("222", 10);

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());

        Assert.Contains("VENCEDOR: Maria Silva (número 10) com 2 voto(s).", linhas);
    }

    [Fact]
    public void GerarLinhas_ComEmpate_ExibeTodosOsEmpatados()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.Votar("111", 10);
        manager.Votar("222", 20);

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());
        var linhaEmpate = linhas.Single(l => l.StartsWith("EMPATE entre:", StringComparison.Ordinal));

        Assert.Contains("Maria Silva (número 10)", linhaEmpate);
        Assert.Contains("João Souza (número 20)", linhaEmpate);
    }

    [Fact]
    public void GerarLinhas_SemVotosComputados_NaoDeclaraVencedor()
    {
        var manager = CriarEleicaoComDoisCandidatos();

        var linhas = ApuracaoReport.GerarLinhas(manager.Apurar());

        Assert.Contains("Eleição sem votos computados: não há vencedor.", linhas);
        Assert.DoesNotContain(linhas, l => l.StartsWith("VENCEDOR:", StringComparison.Ordinal));
    }

    [Fact]
    public void Renderizar_ConcatenaAsLinhasDoRelatorioComCabecalho()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.Votar("111", 10);

        var relatorio = ApuracaoReport.Renderizar(manager.Apurar());

        Assert.Contains("APURAÇÃO FINAL", relatorio);
        Assert.Contains("VENCEDOR: Maria Silva", relatorio);
    }

    [Fact]
    public void GerarLinhas_ResultadoNulo_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ApuracaoReport.GerarLinhas(null!));
    }
}
