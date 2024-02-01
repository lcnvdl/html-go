using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class ClearCmd : INativeInstruction
{
  public string Key => "Clear";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.Clear();
    }
  }
}