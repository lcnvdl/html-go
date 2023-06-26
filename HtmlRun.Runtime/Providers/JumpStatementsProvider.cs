using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class JumpStatementsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new GotoCmd(), new GotoLineCmd(), new LabelCmd(), };
}

class LabelCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Label;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }
}

class GotoCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Goto;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpToLine(ctx.GetArgument()!, JumpToLine.JumpTypeEnum.LineId, ctx.GetArgument(1) == null ? 0 : int.Parse(ctx.GetRequiredArgument(1))));
    }
  }
}

class GotoLineCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.GotoLine;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpToLine(ctx.GetArgument<int>()));
    }
  }
}