using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class ConditionalProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new IfCmd(), new EndIfCmd(), new TestGotoCmd() };
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

class TestGotoCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.TestAndGoto;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var condition = ctx.GetRequiredArgument<bool>();
        var expectedCondition = ctx.GetRequiredArgument<bool>(1);
        string label = ctx.GetRequiredArgument(2);
        int offset = ctx.GetArgument(3) == null ? 0 : int.Parse(ctx.GetRequiredArgument(3));

        if (condition == expectedCondition)
        {
          ctx.Jump(new JumpToLine(label, JumpToLine.JumpTypeEnum.LineId, offset));
        }
      };
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
