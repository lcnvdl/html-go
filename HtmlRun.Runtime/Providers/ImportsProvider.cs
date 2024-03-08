using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class ImportsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new UsingCmd(), new ImportCmd(), };
}

class ImportCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Import;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      //  Empty because is replaced after compilation.
      return ctx => { };
    }
  }
}

class UsingCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.Using;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var unsafeCtx = (IUnsafeCurrentInstructionContext)ctx;
        string namesp = ctx.GetRequiredArgument();

        if (!unsafeCtx.Runtime.Namespaces.Contains(namesp))
        {
          throw new Exception($"Namespace {namesp} not found.");
        }

        unsafeCtx.AddUsing(namesp);
      };
    }
  }
}
