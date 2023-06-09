using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class ImportsProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new UsingCmd(), };
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
