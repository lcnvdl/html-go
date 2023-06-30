using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class IterationStatementsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new WhileCmd(), new DoWhileCmd(), new ForCmd(), };
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

class DoWhileCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.DoWhile;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      //  Empty because is replaced for TestGoto after compilation.
      return ctx => { };
    }
  }
}

class ForCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.For;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      //  Empty because is replaced for TestGoto after compilation.
      return ctx => { };
    }
  }
}