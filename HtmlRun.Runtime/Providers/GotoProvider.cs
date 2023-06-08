using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class GotoProvider : INativeProvider
{
  public string Namespace => Runtime.Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new GotoCmd(), new GotoLineCmd(), new LabelCmd(), };
}

class LabelCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Label;

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
  public string Key => Runtime.Constants.BasicInstructionsSet.Goto;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpToLine(ctx.GetArgument()!, JumpToLine.JumpTypeEnum.LineId));
    }
  }
}

class GotoLineCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.GotoLine;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpToLine(ctx.GetArgument<int>().ToString(), JumpToLine.JumpTypeEnum.LineNumber));
    }
  }
}