using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.Native;

public abstract class VoidInstruction : INativeInstruction
{
  public abstract string Key { get; }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }
}