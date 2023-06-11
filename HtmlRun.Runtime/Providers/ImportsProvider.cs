using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class ImportsProvider : INativeProvider
{
  public string Namespace => Runtime.Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new UsingCmd(), };
}

class UsingCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Using;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var unsafeCtx = ((IUnsafeCurrentInstructionContext)ctx);
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
