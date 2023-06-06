using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class ThreadingProvider : INativeProvider
{
  public string Namespace => "Threading";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new SleepCmd(), };
}

class SleepCmd : INativeInstruction
{
  public string Key => Runtime.Constants.ThreadingInstructionsSet.Sleep;

  public Action<IRuntimeContext> Action
  {
    get
    {
      return ctx => Thread.Sleep(ctx.GetArgument<int>());
    }
  }
}
