using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class VariablesProvider : INativeProvider
{
  public string Namespace => Runtime.Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new SetCmd(), new ConstCmd(), new VarCmd(), new DeleteCmd(), };
}

class SetCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Set;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.SetVariable(ctx.GetRequiredArgument(0), ctx.GetArgument(1)!);
    }
  }
}

class ConstCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Const;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.DeclareAndSetConst(ctx.GetRequiredArgument(0), ctx.GetArgument(1)!);
    }
  }
}

class VarCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Var;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        ctx.DeclareVariable(ctx.GetRequiredArgument());

        if (ctx.CountArguments() > 1)
        {
          ctx.SetVariable(ctx.GetRequiredArgument(), ctx.GetRequiredArgument(1));
        }
      };
    }
  }
}

class DeleteCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Delete;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => ctx.DeleteVariable(ctx.GetRequiredArgument());
    }
  }
}
