using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class ReadKeyCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "ReadKey";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.ReadKey();
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<int>(() => (int)Console.ReadKey().Key);
  }
}