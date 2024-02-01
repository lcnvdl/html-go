using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class HideCursorCmd : INativeInstruction
{
  public string Key => "HideCursor";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.CursorVisible = false;
    }
  }
}