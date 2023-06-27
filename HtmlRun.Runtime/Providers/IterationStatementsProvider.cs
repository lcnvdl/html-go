using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class IterationStatementsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new WhileCmd(), };
}

class WhileCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.While;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      //  Empty because is replaced for TestGoto after compilation.
      return ctx => { };
    }
  }
}
