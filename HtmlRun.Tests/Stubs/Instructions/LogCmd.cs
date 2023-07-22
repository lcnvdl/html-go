using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Tests.Stubs.Instructions;

class LogCmd : INativeInstruction, INativeJSInstruction
{
  public List<string> Logs { get; private set; } = new();

  public string Key => HtmlRun.Runtime.Constants.BasicInstructionsSet.Log;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Logs.Add(string.Join(" ", ctx.GetArgumentsValues().Select(m => m == null ? HtmlRun.Runtime.Constants.Strings.Null : m.ToString())));
    }
  }

  public LogCmd(List<string> logs)
  {
    this.Logs = logs;
  }

  public Delegate ToJSAction()
  {
    return new Action<string>(msg => Logs.Add(msg));
  }
}