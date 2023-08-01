using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class JumpStatementsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new GotoCmd(),
    new GotoLineCmd(),
    new LabelCmd(),
    new ContextPullCmd(),
    new ContextPushCmd(),
    new CallCmd(),
    new ReturnCmd(),
    new PopArgumentsCmd(),
  };
}



class CallCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Call;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpToLineWithCallStack(ctx.GetArgument()!, JumpToLine.JumpTypeEnum.LineId, ctx.GetArgument(1) == null ? 0 : int.Parse(ctx.GetRequiredArgument(1))));
    }
  }
}

class ReturnCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Return;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new JumpReturn());
    }
  }
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

class PopArgumentsCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.PopArguments;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var args = ctx.PopArgumentsAndValues();
        if (args.Arguments == null)
        {
          throw new InvalidOperationException($"Error trying to read arguments of group {args.Label}.");
        }

        foreach (var arg in args.Arguments)
        {
          ctx.DeclareVariable(arg.GroupArgumentName);
          ctx.SetValueVariable(arg.GroupArgumentName, arg.Value);
        }
      };
    }
  }
}

class ContextPushCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.ContextPush;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new ContextPush());
    }
  }
}

class ContextPullCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.ContextPull;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.Jump(new ContextPull());
    }
  }
}