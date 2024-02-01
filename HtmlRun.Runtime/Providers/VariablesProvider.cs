using HtmlRun.Runtime.Exceptions;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class VariablesProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new SetCmd(), new SwapCmd(), new ConstCmd(), new VarCmd(), new DeleteCmd(), };
}

class SetCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Set;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.SetValueVariable(ctx.GetRequiredArgument(0), ctx.GetArgument(1)!);
    }
  }
}

class SwapCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Swap;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var variable1 = ctx.GetVariable(ctx.GetRequiredArgument(0)) ?? throw new VariableNotFoundException(ctx.GetRequiredArgument(0));
        var variable2 = ctx.GetVariable(ctx.GetRequiredArgument(1)) ?? throw new VariableNotFoundException(ctx.GetRequiredArgument(1));

        var val1 = variable1.Value;
        var val2 = variable2.Value;

        ctx.SetValueVariable(ctx.GetRequiredArgument(0), val2);
        ctx.SetValueVariable(ctx.GetRequiredArgument(1), val1);
      };
    }
  }
}

class ConstCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Const;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        ctx.DeclareAndSetConst(ctx.GetRequiredArgument(0), ctx.GetArgument(1)!);

        if (ctx.CountArguments() > 2)
        {
          if (!ctx.GetRequiredArgument(2).Equals("export", StringComparison.InvariantCultureIgnoreCase))
          {
            throw new InvalidDataException();
          }

          ctx.ExportVariable(ctx.GetRequiredArgument());
        }
      };
    }
  }
}

class VarCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Var;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        ctx.DeclareVariable(ctx.GetRequiredArgument());

        if (ctx.CountArguments() > 1)
        {
          ctx.SetValueVariable(ctx.GetRequiredArgument(), ctx.GetRequiredArgument(1));
        }

        if (ctx.CountArguments() > 2)
        {
          if (!ctx.GetRequiredArgument(2).Equals("export", StringComparison.InvariantCultureIgnoreCase))
          {
            throw new InvalidDataException();
          }

          ctx.ExportVariable(ctx.GetRequiredArgument());
        }
      };
    }
  }
}

class DeleteCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Delete;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.DeleteVariable(ctx.GetRequiredArgument());
    }
  }
}
