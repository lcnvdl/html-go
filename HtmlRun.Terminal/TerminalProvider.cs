using HtmlRun.Runtime.Native;
using HtmlRun.Terminal.Instructions;

namespace HtmlRun.Terminal;

class TerminalProvider : INativeProvider
{
  public string Namespace => Runtime.Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new LogCmd(), new SetTitleCmd(), };
}