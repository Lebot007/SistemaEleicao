using SistemaEleicao.Console;

var manager = new EleicaoManager();
var opcao = -1;

while (opcao != 0)
{
    ExibirMenu();
    var linha = Console.ReadLine();
    if (linha is null)
    {
        Console.WriteLine("Entrada encerrada. Saindo do sistema.");
        break;
    }

    if (!int.TryParse(linha.Trim(), out opcao))
    {
        Console.WriteLine("Opção inválida. Digite um número inteiro.");
        continue;
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
        case 0:
            Console.WriteLine("Encerrando o sistema de eleição. Até logo!");
            break;
        default:
            Console.WriteLine("Opção inexistente. Escolha uma opção do menu.");
            break;
    }
}

void ExibirMenu()
{
    Console.WriteLine();
    Console.WriteLine("========== SISTEMA DE ELEIÇÃO ==========");
    Console.WriteLine("1 - Cadastrar candidato");
    Console.WriteLine("2 - Registrar eleitor");
    Console.WriteLine("3 - Votar");
    Console.WriteLine("4 - Apurar resultados");
    Console.WriteLine("0 - Sair");
    Console.Write("Escolha uma opção: ");
}

void CadastrarCandidato()
{
    Console.Write("Nome do candidato: ");
    var nome = Console.ReadLine();
    if (nome is null) return;

    Console.Write("Número do candidato: ");
    if (!LerInteiro(out var numero)) return;

    try
    {
        var candidato = manager.CadastrarCandidato(nome, numero);
        Console.WriteLine($"Candidato {candidato.Nome} (número {candidato.Numero}) cadastrado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao cadastrar candidato: {ex.Message}");
    }
}

void RegistrarEleitor()
{
    Console.Write("Número do título/CPF do eleitor (simulado): ");
    var titulo = Console.ReadLine();
    if (titulo is null) return;

    Console.WriteLine(manager.RegistrarEleitor(titulo)
        ? "Eleitor registrado com sucesso."
        : "Não foi possível registrar: título/CPF vazio ou já registrado.");
}

void Votar()
{
    Console.Write("Número do título/CPF do eleitor: ");
    var titulo = Console.ReadLine();
    if (titulo is null) return;

    Console.Write($"Número do candidato escolhido ({EleicaoManager.NumeroVotoBranco} para voto em branco): ");
    if (!LerInteiro(out var numero)) return;

    try
    {
        var tipoVoto = manager.Votar(titulo, numero);
        Console.WriteLine(tipoVoto switch
        {
            TipoVoto.Candidato => "Voto computado ao candidato.",
            TipoVoto.Branco => "Voto em BRANCO computado.",
            _ => "Voto NULO computado (candidato inexistente)."
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao votar: {ex.Message}");
    }
}

void Apurar()
{
    var resultado = manager.Apurar();

    Console.WriteLine();
    Console.WriteLine("========== APURAÇÃO FINAL ==========");
    if (resultado.Classificacao.Count == 0)
    {
        Console.WriteLine("Nenhum candidato cadastrado.");
    }

    foreach (var candidato in resultado.Classificacao)
    {
        Console.WriteLine($"{candidato.Numero} - {candidato.Nome}: {candidato.Votos} voto(s)");
    }

    Console.WriteLine($"Votos brancos: {resultado.VotosBrancos}");
    Console.WriteLine($"Votos nulos: {resultado.VotosNulos}");

    if (resultado.Vencedores.Count == 0)
        Console.WriteLine("Eleição sem votos computados: não há vencedor.");
    else if (resultado.Vencedores.Count > 1)
        Console.WriteLine($"EMPATE entre: {string.Join(", ", resultado.Vencedores.Select(v => $"{v.Nome} (número {v.Numero})"))}");
    else
        Console.WriteLine($"VENCEDOR: {resultado.Vencedores[0].Nome} (número {resultado.Vencedores[0].Numero}) com {resultado.Vencedores[0].Votos} voto(s).");
}

bool LerInteiro(out int valor)
{
    var linha = Console.ReadLine();
    if (linha is null || !int.TryParse(linha.Trim(), out valor))
    {
        valor = 0;
        Console.WriteLine("Número inválido. Operação cancelada.");
        return false;
    }

    return true;
}
