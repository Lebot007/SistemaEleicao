using SistemaEleicao.Core;
using SistemaEleicao.Ui;
using Xunit;

namespace SistemaEleicao.Tests;

/// <summary>
/// Etapa 3 - Testes unitários do menu interativo da urna.
/// <see cref="UrnaConsole"/> recebe <see cref="TextReader"/> e
/// <see cref="TextWriter"/> por injeção, permitindo exercitar o fluxo
/// completo do console (cadastrar, registrar, votar, apurar, listar)
/// com uma entrada scriptada e sem depender do <see cref="Console"/> real.
/// </summary>
public class UrnaConsoleTests
{
    private static EleicaoManager CriarEleicaoComDoisCandidatos()
    {
        var manager = new EleicaoManager();
        manager.CadastrarCandidato("Maria Silva", 10);
        manager.CadastrarCandidato("João Souza", 20);
        return manager;
    }

    private static (UrnaConsole Urna, StringWriter Saida) CriarUrna(
        EleicaoManager manager, params string[] entradas)
    {
        var saida = new StringWriter();
        var urna = new UrnaConsole(
            manager,
            new StringReader(string.Join(Environment.NewLine, entradas)),
            saida);

        return (urna, saida);
    }

    [Fact]
    public void Executar_MenuExibeTodasAsOpcoesIncluindoListagemEResumo()
    {
        var (urna, saida) = CriarUrna(new EleicaoManager(), "0");

        urna.Executar();
        var tela = saida.ToString();

        Assert.Contains("========== SISTEMA DE ELEIÇÃO ==========", tela);
        Assert.Contains("1 - Cadastrar candidato", tela);
        Assert.Contains("2 - Registrar eleitor", tela);
        Assert.Contains("3 - Votar", tela);
        Assert.Contains("4 - Apurar resultados", tela);
        Assert.Contains("5 - Listar candidatos", tela);
        Assert.Contains("6 - Resumo parcial da eleição", tela);
        Assert.Contains("0 - Sair", tela);
    }

    [Fact]
    public void Executar_OpcaoSair_ExibeMensagemDeDespedida()
    {
        var (urna, saida) = CriarUrna(new EleicaoManager(), "0");

        urna.Executar();

        Assert.Contains("Encerrando o sistema de eleição. Até logo!", saida.ToString());
    }

    [Fact]
    public void Executar_OpcaoNaoNumerica_ExibeOpcaoInvalida()
    {
        var (urna, saida) = CriarUrna(new EleicaoManager(), "abc", "0");

        urna.Executar();

        Assert.Contains("Opção inválida. Digite um número inteiro.", saida.ToString());
    }

    [Fact]
    public void Executar_OpcaoForaDoMenu_ExibeOpcaoInexistente()
    {
        var (urna, saida) = CriarUrna(new EleicaoManager(), "9", "0");

        urna.Executar();

        Assert.Contains("Opção inexistente. Escolha uma opção do menu.", saida.ToString());
    }

    [Fact]
    public void Executar_FluxoCompleto_CadastraRegistraVotaEApura()
    {
        var manager = new EleicaoManager();
        var (urna, saida) = CriarUrna(
            manager,
            "1", "Maria Silva", "10",
            "2", "111",
            "3", "111", "10",
            "4",
            "0");

        urna.Executar();
        var tela = saida.ToString();

        Assert.Contains("Candidato Maria Silva (número 10) cadastrado com sucesso.", tela);
        Assert.Contains("Eleitor registrado com sucesso.", tela);
        Assert.Contains("Voto computado ao candidato.", tela);
        Assert.Contains("VENCEDOR: Maria Silva (número 10) com 1 voto(s).", tela);
        Assert.Contains("Encerrando o sistema de eleição. Até logo!", tela);
        Assert.Equal(1, manager.Candidatos.Single(c => c.Numero == 10).Votos);
    }

    [Fact]
    public void CadastrarCandidato_NumeroDuplicado_ExibeMensagemDeErro()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        var (urna, saida) = CriarUrna(manager, "1", "Pedro Santos", "10", "0");

        urna.Executar();

        Assert.Contains("Erro ao cadastrar candidato:", saida.ToString());
        Assert.Single(manager.Candidatos, c => c.Numero == 10);
    }

    [Fact]
    public void CadastrarCandidato_NumeroNaoInformado_CancelaOperacao()
    {
        var manager = new EleicaoManager();
        var (urna, saida) = CriarUrna(manager, "1", "Maria Silva", "abc", "0");

        urna.Executar();

        Assert.Contains("Número inválido. Operação cancelada.", saida.ToString());
        Assert.Empty(manager.Candidatos);
    }

    [Fact]
    public void RegistrarEleitor_TituloVazio_ExibeMensagemEspecificaDeTituloInvalido()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        var (urna, saida) = CriarUrna(manager, "2", "   ", "0");

        urna.Executar();

        Assert.Contains("Título/CPF inválido: informe um valor não vazio.", saida.ToString());
        Assert.Equal(0, manager.TotalEleitoresRegistrados);
    }

    [Fact]
    public void RegistrarEleitor_Duplicado_ExibeMensagemEspecificaDeEleitorJaRegistrado()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        var (urna, saida) = CriarUrna(manager, "2", "111", "2", "111", "0");

        urna.Executar();

        Assert.Contains("Eleitor já registrado: 111.", saida.ToString());
        Assert.Equal(1, manager.TotalEleitoresRegistrados);
    }

    [Fact]
    public void Votar_EleitorNaoRegistrado_ExibeMensagemDeErro()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        var (urna, saida) = CriarUrna(manager, "3", "999", "10", "0");

        urna.Executar();

        Assert.Contains("Erro ao votar:", saida.ToString());
    }

    [Fact]
    public void Votar_NumeroZero_ExibeVotoEmBranco()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        var (urna, saida) = CriarUrna(manager, "3", "111", "0", "0");

        urna.Executar();

        Assert.Contains("Voto em BRANCO computado.", saida.ToString());
        Assert.Equal(1, manager.VotosBrancos);
    }

    [Fact]
    public void Votar_CandidatoInexistente_ExibeVotoNulo()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        var (urna, saida) = CriarUrna(manager, "3", "111", "99", "0");

        urna.Executar();

        Assert.Contains("Voto NULO computado (candidato inexistente).", saida.ToString());
        Assert.Equal(1, manager.VotosNulos);
    }

    [Fact]
    public void ListarCandidatos_SemCadastro_IndicaListaVazia()
    {
        var (urna, saida) = CriarUrna(new EleicaoManager(), "5", "0");

        urna.Executar();

        Assert.Contains("========== CANDIDATOS CADASTRADOS ==========", saida.ToString());
        Assert.Contains("Nenhum candidato cadastrado.", saida.ToString());
    }

    [Fact]
    public void ListarCandidatos_ComVotos_ExibeNumeroNomeEVotosDeCadaUm()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.Votar("111", 10);
        var (urna, saida) = CriarUrna(manager, "5", "0");

        urna.Executar();
        var tela = saida.ToString();

        Assert.Contains("10 - Maria Silva: 1 voto(s)", tela);
        Assert.Contains("20 - João Souza: 0 voto(s)", tela);
        Assert.Contains("Total de candidatos: 2", tela);
    }

    [Fact]
    public void ResumoParcial_ExibeEleitoresEVotosPorCategoria()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.RegistrarEleitor("333");
        manager.RegistrarEleitor("444");
        manager.Votar("111", 10);
        manager.Votar("222", 0);
        manager.Votar("333", 99);
        var (urna, saida) = CriarUrna(manager, "6", "0");

        urna.Executar();
        var tela = saida.ToString();

        Assert.Contains("========== RESUMO PARCIAL ==========", tela);
        Assert.Contains("Eleitores registrados: 4", tela);
        Assert.Contains("Votos válidos: 1", tela);
        Assert.Contains("Votos brancos: 1", tela);
        Assert.Contains("Votos nulos: 1", tela);
        Assert.Contains("Votos computados: 3", tela);
        Assert.Contains("Ainda não votaram: 1", tela);
    }

    [Fact]
    public void Executar_EntradaEncerrada_SaiDoLoopSemTravar()
    {
        var (urna, saida) = CriarUrna(new EleicaoManager());

        urna.Executar();

        Assert.Contains("Entrada encerrada. Saindo do sistema.", saida.ToString());
    }
}
