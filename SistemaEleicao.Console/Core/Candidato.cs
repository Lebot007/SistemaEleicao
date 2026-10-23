namespace SistemaEleicao.Core;

/// <summary>
/// Representa um candidato cadastrado na eleição.
/// O número é a chave de identificação do candidato na urna.
/// </summary>
public class Candidato
{
    public Candidato(string nome, int numero)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do candidato é obrigatório.", nameof(nome));
        if (numero <= 0)
            throw new ArgumentException("O número do candidato deve ser um inteiro positivo.", nameof(numero));

        Nome = nome.Trim();
        Numero = numero;
    }

    /// <summary>Nome do candidato.</summary>
    public string Nome { get; }

    /// <summary>Número de urna do candidato.</summary>
    public int Numero { get; }

    /// <summary>Total de votos válidos recebidos pelo candidato.</summary>
    public int Votos { get; private set; }

    /// <summary>Registra um voto válido para o candidato.</summary>
    public void RegistrarVoto() => Votos++;
}
