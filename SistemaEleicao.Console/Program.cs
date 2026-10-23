using SistemaEleicao.Core;
using SistemaEleicao.Ui;

// Ponto de entrada da aplicação: cria o domínio da eleição e delega toda a
// interação com o usuário para a camada de apresentação (UrnaConsole), que
// recebe a entrada e a saída por injeção e é coberta por testes unitários.
var manager = new EleicaoManager();
var urna = new UrnaConsole(manager, Console.In, Console.Out);

urna.Executar();
