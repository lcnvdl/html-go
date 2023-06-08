using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.Native;

public class VoidInstruction : INativeInstruction
{
  public string Key { get; private set; }

  public VoidInstruction(string key)
  {
    this.Key = key;
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }
}