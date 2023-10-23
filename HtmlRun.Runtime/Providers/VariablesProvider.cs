using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Exceptions;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class VariablesProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new SetCmd(), new SwapCmd(), new ConstCmd(), new VarCmd(), new DeleteCmd(), };
}

abstract class VariableBaseCmd
{
  protected bool ShouldBeSavedAsPointer(ParsedArgument parsedArgument)
  {
    return parsedArgument.RawValue != null && parsedArgument.RawValue!.GetType()!.FullName!.Contains("Jurassic");
  }

  protected void SetValueToContext(ICurrentInstructionContext ctx, ParsedArgument parsedArgument, string varName)
  {
    if (this.ShouldBeSavedAsPointer(parsedArgument))
    {
      var rawValue = parsedArgument.RawValue!;
      ctx.AllocAndSetPointerVariable(varName, rawValue);
    }
    else
    {
      ctx.SetValueVariable(varName, parsedArgument.Value);
    }
  }
}

class SetCmd : VariableBaseCmd, INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Set;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        this.SetValueToContext(ctx, ctx.GetArgumentAt(1), ctx.GetRequiredArgument());
      };
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

        if (variable1.IsConst || variable2.IsConst)
        {
          throw new InvalidOperationException();
        }

        var val1 = variable1.Value;
        var val2 = variable2.Value;

        bool isVar1Pointer = variable1.IsPointer;
        bool isVar2Pointer = variable2.IsPointer;

        ctx.SetValueVariable(ctx.GetRequiredArgument(0), val2);
        ctx.SetValueVariable(ctx.GetRequiredArgument(1), val1);

        variable1.IsPointer = isVar2Pointer;
        variable2.IsPointer = isVar1Pointer;
      };
    }
  }
}

class ConstCmd : VariableBaseCmd, INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Const;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        //  TODO  Improve DeclareAndSetConst in order to uncomment it and remove the next lines
        // ctx.DeclareAndSetConst(ctx.GetRequiredArgument(0), ctx.GetArgument(1)!);

        string varName = ctx.GetRequiredArgument();

        ctx.DeclareVariable(varName);

        var parsedArgument = ctx.GetArgumentAt(1);

        this.SetValueToContext(ctx, parsedArgument, varName);

        ctx.GetVariable(varName)!.IsConst = true;
      };
    }
  }
}

class VarCmd : VariableBaseCmd, INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Var;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string varName = ctx.GetRequiredArgument();

        ctx.DeclareVariable(varName);

        if (ctx.CountArguments() > 1)
        {
          var parsedArgument = ctx.GetArgumentAt(1);

          this.SetValueToContext(ctx, parsedArgument, varName);
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
