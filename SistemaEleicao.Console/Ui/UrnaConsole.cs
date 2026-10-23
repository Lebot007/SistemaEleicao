using SistemaEleicao.Core;

namespace SistemaEleicao.Ui;

/// <summary>
/// Camada de interação da urna eletrônica: exibe o menu, lê as escolhas do
/// usuário e aciona o <see cref="EleicaoManager"/>. A entrada e a saída são
/// recebidas por injeção (<see cref="TextReader"/>/<see cref="TextWriter"/>),
/// o que permite testar todo o fluxo do console com uma entrada scriptada.
/// </summary>
public sealed class UrnaConsole
{
    /// <summary>Opção que encerra o sistema.</summary>
    public const int OpcaoSair = 0;

    private const string CabecalhoMenu = "========== SISTEMA DE ELEIÇÃO ==========";
    private const string CabecalhoCandidatos = "========== CANDIDATOS CADASTRADOS ==========";
    private const string CabecalhoResumo = "========== RESUMO PARCIAL ==========";

    private readonly EleicaoManager _manager;
    private readonly TextReader _entrada;
    private readonly TextWriter _saida;

    public UrnaConsole(EleicaoManager manager, TextReader entrada, TextWriter saida)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        _entrada = entrada ?? throw new ArgumentNullException(nameof(entrada));
        _saida = saida ?? throw new ArgumentNullException(nameof(saida));
    }

    /// <summary>Executa o laço principal do menu até o usuário sair ou a entrada terminar.</summary>
    public void Executar()
    {
        while (true)
        {
            ExibirMenu();

            var linha = _entrada.ReadLine();
            if (linha is null)
            {
                EscreverLinha("Entrada encerrada. Saindo do sistema.");
                return;
            }

            if (!int.TryParse(linha.Trim(), out var opcao))
            {
                EscreverLinha("Opção inválida. Digite um número inteiro.");
                continue;
            }

            if (opcao == OpcaoSair)
            {
                EscreverLinha("Encerrando o sistema de eleição. Até logo!");
                return;
            }

            switch (opcao)
            {
                case 1:
                    CadastrarCandidato();
                    break;
                case 2:
                    RegistrarEleitor();
                    break;
                case 3:
                    Votar();
                    break;
                case 4:
                    Apurar();
                    break;
                case 5:
                    ListarCandidatos();
                    break;
                case 6:
                    ExibirResumoParcial();
                    break;
                default:
                    EscreverLinha("Opção inexistente. Escolha uma opção do menu.");
                    break;
            }
        }
    }

    private void ExibirMenu()
    {
        EscreverLinha(string.Empty);
        EscreverLinha(CabecalhoMenu);
        EscreverLinha("1 - Cadastrar candidato");
        EscreverLinha("2 - Registrar eleitor");
        EscreverLinha("3 - Votar");
        EscreverLinha("4 - Apurar resultados");
        EscreverLinha("5 - Listar candidatos");
        EscreverLinha("6 - Resumo parcial da eleição");
        EscreverLinha($"{OpcaoSair} - Sair");
        Escrever("Escolha uma opção: ");
    }

    private void CadastrarCandidato()
    {
        Escrever("Nome do candidato: ");
        var nome = _entrada.ReadLine();
        if (nome is null) return;

        Escrever("Número do candidato: ");
        if (!LerInteiro(out var numero)) return;

        try
        {
            var candidato = _manager.CadastrarCandidato(nome, numero);
            EscreverLinha($"Candidato {candidato.Nome} (número {candidato.Numero}) cadastrado com sucesso.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            EscreverLinha($"Erro ao cadastrar candidato: {ex.Message}");
        }
    }

    private void RegistrarEleitor()
    {
        Escrever("Número do título/CPF do eleitor (simulado): ");
        var titulo = _entrada.ReadLine();
        if (titulo is null) return;

        if (string.IsNullOrWhiteSpace(titulo))
        {
            EscreverLinha("Título/CPF inválido: informe um valor não vazio.");
            return;
        }

        EscreverLinha(_manager.RegistrarEleitor(titulo)
            ? "Eleitor registrado com sucesso."
            : $"Eleitor já registrado: {titulo.Trim()}.");
    }

    private void Votar()
    {
        Escrever("Número do título/CPF do eleitor: ");
        var titulo = _entrada.ReadLine();
        if (titulo is null) return;

        Escrever($"Número do candidato escolhido ({EleicaoManager.NumeroVotoBranco} para voto em branco): ");
        if (!LerInteiro(out var numero)) return;

        try
        {
            var tipoVoto = _manager.Votar(titulo, numero);
            EscreverLinha(tipoVoto switch
            {
                TipoVoto.Candidato => "Voto computado ao candidato.",
                TipoVoto.Branco => "Voto em BRANCO computado.",
                _ => "Voto NULO computado (candidato inexistente)."
            });
        }
        catch (InvalidOperationException ex)
        {
            EscreverLinha($"Erro ao votar: {ex.Message}");
        }
    }

    private void Apurar()
    {
        EscreverLinha(string.Empty);
        EscreverLinha(ApuracaoReport.Renderizar(_manager.Apurar()));
    }

    private void ListarCandidatos()
    {
        EscreverLinha(string.Empty);
        EscreverLinha(CabecalhoCandidatos);

        var candidatos = _manager.Candidatos;
        if (candidatos.Count == 0)
        {
            EscreverLinha(ApuracaoReport.SemCandidatos);
            return;
        }

        foreach (var candidato in candidatos)
            EscreverLinha($"{candidato.Numero} - {candidato.Nome}: {candidato.Votos} voto(s)");

        EscreverLinha($"Total de candidatos: {candidatos.Count}");
    }

    private void ExibirResumoParcial()
    {
        var votosValidos = _manager.Candidatos.Sum(c => c.Votos);
        var votosComputados = votosValidos + _manager.VotosBrancos + _manager.VotosNulos;
        var pendentes = Math.Max(_manager.TotalEleitoresRegistrados - votosComputados, 0);

        EscreverLinha(string.Empty);
        EscreverLinha(CabecalhoResumo);
        EscreverLinha($"Eleitores registrados: {_manager.TotalEleitoresRegistrados}");
        EscreverLinha($"Votos válidos: {votosValidos}");
        EscreverLinha($"Votos brancos: {_manager.VotosBrancos}");
        EscreverLinha($"Votos nulos: {_manager.VotosNulos}");
        EscreverLinha($"Votos computados: {votosComputados}");
        EscreverLinha($"Ainda não votaram: {pendentes}");
    }

    private bool LerInteiro(out int valor)
    {
        var linha = _entrada.ReadLine();
        if (linha is null || !int.TryParse(linha.Trim(), out valor))
        {
            valor = 0;
            EscreverLinha("Número inválido. Operação cancelada.");
            return false;
        }

        return true;
    }

    private void Escrever(string texto) => _saida.Write(texto);

    private void EscreverLinha(string texto) => _saida.WriteLine(texto);
}
