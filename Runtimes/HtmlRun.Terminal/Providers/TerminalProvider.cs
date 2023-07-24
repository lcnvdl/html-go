using HtmlRun.Runtime.Native;
using HtmlRun.Terminal.Instructions;

namespace HtmlRun.Terminal;

class TerminalProvider : INativeProvider
{
  public string Namespace => "Console";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new ReadLineCmd(), };
}