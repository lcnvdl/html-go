using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class LogCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.Log;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Console.WriteLine(string.Join(" ", ctx.GetArgumentsValues().Select(m => m == null ? Runtime.Constants.Strings.Null : m.ToString())));
    }
  }

  public Delegate ToJSAction()
  {
    return new Action<string>(msg => Console.WriteLine(msg));
  }
}