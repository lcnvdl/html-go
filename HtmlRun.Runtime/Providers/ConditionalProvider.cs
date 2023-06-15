using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class ConditionalProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new IfCmd(), new EndIfCmd(),};
}

class IfCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.If;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpToBranch(ctx.GetArgument<bool>().ToString()));
    }
  }
}

class EndIfCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.EndIf;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }
}
