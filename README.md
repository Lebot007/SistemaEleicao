# 🗳️ SistemaEleicao

**Lista de Exercícios — Git + Conventional Commits + .NET (Console + xUnit)**

| | |
|---|---|
| **Disciplina** | Garantia da Qualidade de Software / Gestão e Qualidade de Software |
| **Professor** | Daniel Henrique Matos de Paiva |
| **Aluno** | João Vitor Alves Rodrigues |
| **RA** | 32513480 |
| **GitHub** | [@Lebot007](https://github.com/Lebot007) |
| **Equipe** | Trabalho em dupla (autorizado pelo professor): a lista foi dividida em **Parte 1** (Etapas 1 e 2 — regras de negócio de candidatos, eleitores e votação) e **Parte 2** (Etapas 3 e 4 — interface de console, refatoração e documentação), com os commits das partes distribuídos no histórico deste repositório. |

---

## 📋 Requisitos Funcionais da Aplicação

1. **Cadastrar Candidatos** — nome e número do candidato, com validação para evitar números duplicados.
2. **Registrar Eleitores** — número de título/CPF (simulado) para controle de quem já votou.
3. **Votar** — o eleitor vota digitando o número do candidato escolhido (ou voto nulo/branco); o sistema valida se o eleitor já votou anteriormente.
4. **Apuração de Votos** — relatório final com total de votos por candidato, votos nulos, brancos e o candidato vencedor.

## 🧱 Estrutura da Solução

```
SistemaEleicao/
├── SistemaEleicao.sln
├── SistemaEleicao.Console/          # Aplicação de console (domínio + menu)
│   ├── Core/
│   │   ├── Candidato.cs             # Entidade candidato + validações
│   │   ├── EleicaoManager.cs        # Gerência: cadastro, eleitores, votos, apuração
│   │   ├── TipoVoto.cs              # Enum: Candidato | Branco | Nulo
│   │   └── ApuracaoResult.cs        # Record com o resultado da apuração
│   └── Program.cs                   # Menu interativo da urna
└── SistemaEleicao.Tests/            # Testes unitários (xUnit)
    ├── CandidatoTests.cs            # Regras de cadastro de candidatos
    └── VotacaoTests.cs              # Voto duplicado, nulo, branco e apuração
```

## ▶️ Como Rodar

**Pré-requisito:** [.NET SDK 8.0](https://dotnet.microsoft.com/download) ou superior.

```bash
# Clonar o repositório
git clone https://github.com/Lebot007/SistemaEleicao.git
cd SistemaEleicao

# Executar a aplicação de console (menu interativo)
dotnet run --project SistemaEleicao.Console/SistemaEleicao.Console.csproj

# Executar os testes unitários
dotnet test
```

### Exemplo de sessão no menu

```
========== SISTEMA DE ELEIÇÃO ==========
1 - Cadastrar candidato
2 - Registrar eleitor
3 - Votar
4 - Apurar resultados
0 - Sair
Escolha uma opção: 1
Nome do candidato: Maria Silva
Número do candidato: 10
Candidato Maria Silva (número 10) cadastrado com sucesso.
...
========== APURAÇÃO FINAL ==========
10 - Maria Silva: 2 voto(s)
20 - Joao Souza: 1 voto(s)
Votos brancos: 1
Votos nulos: 1
VENCEDOR: Maria Silva (número 10) com 2 voto(s).
```

## 🔒 Regras de Negócio Implementadas

- **Número de candidato é único:** tentar cadastrar dois candidatos com o mesmo número lança `InvalidOperationException`.
- **Nome obrigatório e número positivo:** entradas inválidas lançam `ArgumentException`.
- **Eleitor registrado uma única vez:** título/CPF duplicado não é registrado novamente.
- **Um eleitor, um voto:** segunda tentativa de voto do mesmo eleitor lança `InvalidOperationException`.
- **Eleitor não registrado não vota.**
- **Voto em número inexistente → voto NULO; número `0` → voto em BRANCO.**
- **Apuração:** candidatos ordenados por votos; empate retorna todos os empatados; sem votos, nenhum vencedor é declarado.

## ✅ Testes Unitários (xUnit)

17 testes cobrindo os cenários exigidos pela lista:

| Classe | Cenários |
|---|---|
| `CandidatoTests` | cadastro válido; número duplicado; número inválido (0/negativo); nome vazio |
| `VotacaoTests` | registro de eleitor (válido/duplicado); eleitor não registrado; **prevenção de voto duplicado**; **voto em candidato inexistente → nulo**; voto em branco (0); voto válido incrementa; apuração com vencedor; empate; apuração sem votos |

```bash
dotnet test
# Passed! - Failed: 0, Passed: 17, Skipped: 0, Total: 17
```

## 📜 Padrão de Commits (Conventional Commits)

Todo o histórico segue estritamente o padrão [Conventional Commits](https://www.conventionalcommits.org/):

| Tipo | Uso no projeto |
|---|---|
| `chore` | estrutura inicial do projeto |
| `test` | testes unitários (escritos antes da implementação — TDD) |
| `feat` | novas funcionalidades (candidatos, votação, menu) |
| `refactor` | refatoração e organização de namespaces |
| `docs` | documentação (este README) |

Histórico da entrega (`git log --oneline`):

```
chore: initial project structure with console and xunit apps
test(candidato): add unit tests for candidate registration rules
feat(candidato): implement candidate entity and validation logic
test(votacao): add unit tests for duplicate voting prevention and invalid votes
feat(votacao): implement voting logic and voter verification
feat(console): implement interactive menu for election management
refactor(core): improve code readability and organize namespaces
docs: add comprehensive instructions in README.md
```

## 🔀 Fluxo Git Utilizado

1. `git init` + estruturação da solução (.NET sln/console/xUnit);
2. desenvolvimento orientado a testes (TDD): commit de `test` (red) seguido do `feat` (green) em cada etapa;
3. branch principal `main` protegida por histórico limpo e semântico;
4. publicação no GitHub com `git remote add origin … && git branch -M main && git push -u origin main`;
5. marcação da entrega final com a tag `v1.0.0`.

---

*Entrega da Lista de Exercícios — Garantia da Qualidade de Software (2026).*
