using SistemaEleicao.Core;
using Xunit;

namespace SistemaEleicao.Tests;

/// <summary>
/// Etapa 2 - Testes unitários do controle de eleitores e votação:
/// prevenção de voto duplicado, contabilização de votos nulos e brancos
/// e apuração com declaração do vencedor.
/// </summary>
public class VotacaoTests
{
    private static EleicaoManager CriarEleicaoComDoisCandidatos()
    {
        var manager = new EleicaoManager();
        manager.CadastrarCandidato("Maria Silva", 10);
        manager.CadastrarCandidato("João Souza", 20);
        return manager;
    }

    [Fact]
    public void RegistrarEleitor_Valido_RetornaVerdadeiro()
    {
        var manager = CriarEleicaoComDoisCandidatos();

        Assert.True(manager.RegistrarEleitor("123456789"));
        Assert.Equal(1, manager.TotalEleitoresRegistrados);
    }

    [Fact]
    public void RegistrarEleitor_Duplicado_RetornaFalso()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("123456789");

        Assert.False(manager.RegistrarEleitor("123456789"));
        Assert.Equal(1, manager.TotalEleitoresRegistrados);
    }

    [Fact]
    public void Votar_EleitorNaoRegistrado_LancaInvalidOperationException()
    {
        var manager = CriarEleicaoComDoisCandidatos();

        Assert.Throws<InvalidOperationException>(() => manager.Votar("999999", 10));
    }

    [Fact]
    public void Votar_EleitorQueJaVotou_LancaInvalidOperationException()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("123456789");
        manager.Votar("123456789", 10);

        Assert.Throws<InvalidOperationException>(() => manager.Votar("123456789", 20));
    }

    [Fact]
    public void Votar_CandidatoInexistente_ContabilizaVotoNulo()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("123456789");

        var tipoVoto = manager.Votar("123456789", 99);

        Assert.Equal(TipoVoto.Nulo, tipoVoto);
        Assert.Equal(1, manager.VotosNulos);
    }

    [Fact]
    public void Votar_NumeroZero_ContabilizaVotoBranco()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("123456789");

        var tipoVoto = manager.Votar("123456789", EleicaoManager.NumeroVotoBranco);

        Assert.Equal(TipoVoto.Branco, tipoVoto);
        Assert.Equal(1, manager.VotosBrancos);
    }

    [Fact]
    public void Votar_CandidatoValido_IncrementaVotosDoCandidato()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("123456789");

        var tipoVoto = manager.Votar("123456789", 10);

        Assert.Equal(TipoVoto.Candidato, tipoVoto);
        Assert.Equal(1, manager.Candidatos.First(c => c.Numero == 10).Votos);
    }

    [Fact]
    public void Apurar_DevolveVencedorComMaisVotos()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.RegistrarEleitor("333");
        manager.Votar("111", 10);
        manager.Votar("222", 10);
        manager.Votar("333", 20);

        var resultado = manager.Apurar();

        Assert.Equal(10, resultado.Classificacao[0].Numero);
        Assert.Single(resultado.Vencedores);
        Assert.Equal(10, resultado.Vencedores[0].Numero);
        Assert.Equal(2, resultado.Vencedores[0].Votos);
    }

    [Fact]
    public void Apurar_Empate_DevolveTodosOsCandidatosEmpatados()
    {
        var manager = CriarEleicaoComDoisCandidatos();
        manager.RegistrarEleitor("111");
        manager.RegistrarEleitor("222");
        manager.Votar("111", 10);
        manager.Votar("222", 20);

        var resultado = manager.Apurar();

        Assert.Equal(2, resultado.Vencedores.Count);
    }

    [Fact]
    public void Apurar_SemVotos_NaoDeclaraVencedor()
    {
        var manager = CriarEleicaoComDoisCandidatos();

        var resultado = manager.Apurar();

        Assert.Empty(resultado.Vencedores);
        Assert.Equal(0, resultado.VotosBrancos);
        Assert.Equal(0, resultado.VotosNulos);
    }
}
