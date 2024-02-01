using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class MathProvider : INativeProvider
{
  public string Namespace => "Math";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new ClampCmd(),
  };
}

class ClampCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.MathInstructionsSet.Clamp;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<double, double, double, double>((a, b, c) => Math.Clamp(a, b, c));
  }
}


