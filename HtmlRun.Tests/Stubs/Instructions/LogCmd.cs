using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Tests.Stubs.Instructions;

class LogCmd : INativeInstruction, INativeJSInstruction
{
  public static List<string> Logs { get; private set; } = new();

  public string Key => Runtime.Constants.BasicInstructionsSet.Log;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Logs.Add(string.Join(" ", ctx.GetArgumentsValues().Select(m => m == null ? Runtime.Constants.Strings.Null : m.ToString())));
    }
  }

  public Delegate ToJSAction()
  {
    return new Action<string>(msg => Logs.Add(msg));
  }
}