# 🗳️ SistemaEleicao

**Lista de Exercícios — Git + Conventional Commits + .NET (Console + xUnit)**

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![Testes](https://img.shields.io/badge/testes-42%20passando-brightgreen)
![Versão](https://img.shields.io/badge/versão-v1.1.0-blue)

| | |
|---|---|
| **Disciplina** | Garantia da Qualidade de Software / Gestão e Qualidade de Software |
| **Professor** | Daniel Henrique Matos de Paiva |
| **Equipe** | Trabalho em dupla (autorizado pelo professor) — ver [Divisão da equipe](#-divisão-da-equipe) |
| **Versões** | `v1.0.0` (Parte 1) · `v1.1.0` (Parte 2) |

---

## 👥 Divisão da Equipe

A lista foi dividida em duas partes, cada uma entregue por um integrante no histórico
deste repositório (commits autorais, todos aderentes ao Conventional Commits):

| Parte | Integrante | GitHub | Escopo (etapas da lista) |
|---|---|---|---|
| **Parte 1** | João Vitor Alves Rodrigues (RA 32513480) | [@Lebot007](https://github.com/Lebot007) | **Etapas 1 e 2** — estrutura da solução, entidade `Candidato`, `EleicaoManager`, controle de eleitores, votação e testes de negócio (`Core`) |
| **Parte 2** | Rafael Souza | [@rafaellfsouza](https://github.com/rafaellfsouza) | **Etapas 3 e 4** — interface de console testável, listagem/resumo, refatoração da apresentação, `.editorconfig` e documentação |

> A Parte 2 foi desenvolvida **sobre** o código da Parte 1, sem alterar as regras de
> negócio de `SistemaEleicao.Core`: os 17 testes originais continuam passando sem
> nenhuma modificação.

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
├── .editorconfig                        # Padrões de código da solução        (Parte 2)
├── SistemaEleicao.Console/              # Aplicação de console
│   ├── Core/                            # Domínio e regras de negócio       (Parte 1)
│   │   ├── Candidato.cs                 # Entidade candidato + validações
│   │   ├── EleicaoManager.cs            # Gerência: cadastro, eleitores, votos, apuração
│   │   ├── TipoVoto.cs                  # Enum: Candidato | Branco | Nulo
│   │   └── ApuracaoResult.cs            # Record com o resultado da apuração
│   ├── Ui/                              # Camada de apresentação            (Parte 2)
│   │   ├── ApuracaoReport.cs            # Renderização pura do relatório de apuração
│   │   └── UrnaConsole.cs               # Menu interativo com TextReader/TextWriter injetados
│   └── Program.cs                       # Bootstrap (151 → 10 linhas)       (Parte 2)
└── SistemaEleicao.Tests/                # Testes unitários (xUnit)
    ├── CandidatoTests.cs                # Regras de cadastro de candidatos   (Parte 1)
    ├── VotacaoTests.cs                  # Voto duplicado, nulo, branco, apuração (Parte 1)
    ├── ApuracaoReportTests.cs           # Formatação do relatório final      (Parte 2)
    └── UrnaConsoleTests.cs              # Fluxo do menu com entrada scriptada (Parte 2)
```

### Arquitetura em camadas

```
Program.cs  ──►  Ui/UrnaConsole  ──►  Core/EleicaoManager  ──►  Core/Candidato
                        │                     │
                        └──►  Ui/ApuracaoReport ◄── ApuracaoResult
```

`Program.cs` não contém mais lógica: apenas instancia o domínio e delega a interação
para `UrnaConsole`, que recebe `TextReader`/`TextWriter` por injeção — é isso que
permite testar o console inteiro com uma entrada scriptada, sem `Console` real.

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

### Menu da urna

```
========== SISTEMA DE ELEIÇÃO ==========
1 - Cadastrar candidato
2 - Registrar eleitor
3 - Votar
4 - Apurar resultados
5 - Listar candidatos          ← Parte 2
6 - Resumo parcial da eleição  ← Parte 2
0 - Sair
Escolha uma opção:
```

### Exemplo de sessão

```
Escolha uma opção: 1
Nome do candidato: Maria Silva
Número do candidato: 10
Candidato Maria Silva (número 10) cadastrado com sucesso.

Escolha uma opção: 5
========== CANDIDATOS CADASTRADOS ==========
10 - Maria Silva: 0 voto(s)
20 - Joao Souza: 0 voto(s)
Total de candidatos: 2

Escolha uma opção: 6
========== RESUMO PARCIAL ==========
Eleitores registrados: 3
Votos válidos: 2
Votos brancos: 1
Votos nulos: 0
Votos computados: 3
Ainda não votaram: 0

Escolha uma opção: 4
========== APURAÇÃO FINAL ==========
10 - Maria Silva: 2 voto(s)
20 - Joao Souza: 1 voto(s)
Votos brancos: 1
Votos nulos: 0
VENCEDOR: Maria Silva (número 10) com 2 voto(s).
```

## 🔒 Regras de Negócio Implementadas

**Parte 1 — domínio (`Core`)**

- **Número de candidato é único:** tentar cadastrar dois candidatos com o mesmo número lança `InvalidOperationException`.
- **Nome obrigatório e número positivo:** entradas inválidas lançam `ArgumentException`.
- **Eleitor registrado uma única vez:** título/CPF duplicado não é registrado novamente.
- **Um eleitor, um voto:** segunda tentativa de voto do mesmo eleitor lança `InvalidOperationException`.
- **Eleitor não registrado não vota.**
- **Voto em número inexistente → voto NULO; número `0` → voto em BRANCO.**
- **Apuração:** candidatos ordenados por votos; empate retorna todos os empatados; sem votos, nenhum vencedor é declarado.

**Parte 2 — apresentação (`Ui`)**

- **Relatório desacoplado do `Console`:** `ApuracaoReport` é uma classe pura que converte `ApuracaoResult` em linhas de texto.
- **Console testável:** `UrnaConsole` recebe entrada e saída por injeção.
- **Mensagens de erro específicas:** título/CPF **vazio** e título/CPF **já registrado** produzem mensagens distintas (antes a mesma mensagem cobria os dois casos).
- **Sem loop infinito em EOF:** quando a entrada padrão é encerrada (`Ctrl+D`), o menu é finalizado com aviso.
- **Menu orientado a dados:** as opções são uma tabela `OpcaoMenu(código, rótulo, ação)`; adicionar uma opção não exige tocar no laço principal.
- **Novas consultas:** listagem de candidatos com votação parcial e resumo da eleição (eleitores, válidos, brancos, nulos, computados e pendentes).

## ✅ Testes Unitários (xUnit)

**42 testes** cobrindo os cenários exigidos pela lista:

| Classe | Parte | Cenários |
|---|---|---|
| `CandidatoTests` | 1 | cadastro válido; número duplicado; número inválido (0/negativo); nome vazio |
| `VotacaoTests` | 1 | registro de eleitor (válido/duplicado); eleitor não registrado; **prevenção de voto duplicado**; **voto em candidato inexistente → nulo**; voto em branco (0); voto válido incrementa; apuração com vencedor; empate; apuração sem votos |
| `ApuracaoReportTests` | 2 | relatório sem candidatos; linha por candidato; ordenação decrescente por votos; totais de brancos/nulos; vencedor único; empate; ausência de vencedor; renderização completa; `null` → `ArgumentNullException` |
| `UrnaConsoleTests` | 2 | menu completo; sair; opção não numérica; opção inexistente; **fluxo completo cadastrar→registrar→votar→apurar**; número duplicado; número inválido; título vazio × título duplicado; eleitor não registrado; voto em branco; voto nulo; listagem vazia e com votos; resumo parcial; EOF sem travar |

```bash
dotnet test
# Passed!  - Failed: 0, Passed: 42, Skipped: 0, Total: 42
```

## 📜 Padrão de Commits (Conventional Commits)

Todo o histórico segue estritamente o padrão [Conventional Commits](https://www.conventionalcommits.org/):

| Tipo | Uso no projeto |
|---|---|
| `chore` | estrutura inicial do projeto e `.editorconfig` |
| `test` | testes unitários (escritos antes da implementação — TDD) |
| `feat` | novas funcionalidades (candidatos, votação, menu, listagem, resumo) |
| `refactor` | refatoração e organização de namespaces e da camada de apresentação |
| `docs` | documentação (este README) |

### Histórico da entrega (`git log --oneline`)

**Parte 1 — [@Lebot007](https://github.com/Lebot007) · Etapas 1 e 2 · tag `v1.0.0`**

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

**Parte 2 — [@rafaellfsouza](https://github.com/rafaellfsouza) · Etapas 3 e 4 · tag `v1.1.0`**

```
test(console): add unit tests for election report rendering
feat(console): extract apuration report rendering into ApuracaoReport
test(console): add unit tests for interactive menu flow with scripted input
feat(console): implement testable console shell with candidate listing and partial summary
refactor(console): replace menu switch with a data-driven option table
chore: add editorconfig with C# coding conventions
docs: document team division and Parte 2 delivery in README.md
```

## 🔀 Fluxo Git Utilizado

1. `git init` + estruturação da solução (.NET sln/console/xUnit);
2. desenvolvimento orientado a testes (TDD): commit de `test` (red) seguido do `feat` (green) em cada etapa;
3. branch principal `main` com histórico limpo, linear e semântico;
4. publicação no GitHub com `git remote add origin … && git branch -M main && git push -u origin main`;
5. **Parte 1** entregue e marcada com a tag `v1.0.0`;
6. **Parte 2** desenvolvida sobre a `main`, mantendo os testes da Parte 1 intactos, e marcada com a tag `v1.1.0`;
7. cada commit é atribuível ao seu autor (`git log --format='%an %s'`), evidenciando a divisão da equipe.

### Verificar a autoria de cada parte

```bash
git log --oneline --format='%h %an: %s'
git shortlog -sne            # quantos commits cada integrante fez
git log --author=rafaellfsouza --oneline
```

---

*Entrega da Lista de Exercícios — Garantia da Qualidade de Software (2026).*
