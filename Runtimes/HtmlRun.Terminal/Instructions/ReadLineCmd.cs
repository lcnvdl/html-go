using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class ReadLineCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "ReadLine";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.ReadLine();
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string>(() => Console.ReadLine()!);
  }
}