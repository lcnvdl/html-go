using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class PeekKeyCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "PeekKey";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<int>(() =>
    {
      if (!Console.KeyAvailable)
      {
        return -1;
      }

      int key = (int)Console.ReadKey(true).Key;

      while (Console.KeyAvailable)
      {
        Console.ReadKey(true);
      }

      return key;
    });
  }
}