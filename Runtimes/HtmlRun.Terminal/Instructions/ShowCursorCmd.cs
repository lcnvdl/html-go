using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class ShowCursorCmd : INativeInstruction
{
  public string Key => "ShowCursor";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.CursorVisible = true;
    }
  }
}