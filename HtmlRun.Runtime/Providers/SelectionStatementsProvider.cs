using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class SelectionStatementsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new IfCmd(), new EndIfCmd(), new SwitchCmd(), new EndSwitchCmd(), new TestGotoCmd(), };
}

class IfCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.If;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      //  Empty because is replaced for TestGoto after compilation.
      return ctx => { };
    }
  }
}

class SwitchCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Switch;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      //  Empty because is replaced for TestGoto after compilation.
      return ctx => { };
    }
  }
}

class TestGotoCmd : INativeInstruction, IInternalInstruction
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

class EndIfCmd : VoidInstruction
{
  public EndIfCmd() : base(Constants.BasicInstructionsSet.EndIf)
  {
  }
}


class EndSwitchCmd : VoidInstruction
{
  public EndSwitchCmd() : base(Constants.BasicInstructionsSet.EndSwitch)
  {
  }
}
