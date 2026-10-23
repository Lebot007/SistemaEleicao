using SistemaEleicao.Core;
using Xunit;

namespace SistemaEleicao.Tests;

/// <summary>
/// Etapa 1 - Testes unitários das regras de cadastro de candidatos.
/// Garante, entre outros, que não seja possível cadastrar dois candidatos
/// com o mesmo número.
/// </summary>
public class CandidatoTests
{
    [Fact]
    public void CadastrarCandidato_Valido_AdicionaCandidatoALista()
    {
        var manager = new EleicaoManager();

        var candidato = manager.CadastrarCandidato("Maria Silva", 10);

        Assert.Contains(candidato, manager.Candidatos);
        Assert.Equal("Maria Silva", candidato.Nome);
        Assert.Equal(10, candidato.Numero);
    }

    [Fact]
    public void CadastrarCandidato_NumeroDuplicado_LancaInvalidOperationException()
    {
        var manager = new EleicaoManager();
        manager.CadastrarCandidato("Maria Silva", 10);

        var excecao = Assert.Throws<InvalidOperationException>(
            () => manager.CadastrarCandidato("João Souza", 10));

        Assert.Contains("10", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void CadastrarCandidato_NumeroInvalido_LancaArgumentException(int numero)
    {
        var manager = new EleicaoManager();

        Assert.Throws<ArgumentException>(
            () => manager.CadastrarCandidato("Pedro Santos", numero));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CadastrarCandidato_NomeVazio_LancaArgumentException(string? nome)
    {
        var manager = new EleicaoManager();

        Assert.Throws<ArgumentException>(
            () => manager.CadastrarCandidato(nome!, 20));
    }
}
