using HtmlRun.Runtime.Native;
using HtmlRun.Tests.Stubs.Instructions;

namespace HtmlRun.Tests.Stubs;

class InstructionsProvider : INativeProvider
{
  public string Namespace => Runtime.Constants.Namespaces.Global;

  public List<string> Logs { get; private set; }

  public INativeInstruction[] Instructions => new INativeInstruction[] { new LogCmd(this.Logs), new SetTitleCmd(), };

  public InstructionsProvider(List<string> logs)
  {
    this.Logs = logs;
  }
}