using HtmlRun.Runtime.Native;
using HtmlRun.Tests.Stubs.Instructions;

namespace HtmlRun.Tests.Stubs;

class InstructionsProvider : INativeProvider
{
  public string Namespace => Runtime.Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new LogCmd(), new SetTitleCmd(), };
}