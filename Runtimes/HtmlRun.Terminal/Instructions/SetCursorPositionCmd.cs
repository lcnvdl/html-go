using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class SetCursorPositionCmd : INativeInstruction
{
  public string Key => "SetCursorPosition";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.SetCursorPosition(ctx.GetRequiredArgument<int>(0), ctx.GetRequiredArgument<int>(1));
    }
  }
}