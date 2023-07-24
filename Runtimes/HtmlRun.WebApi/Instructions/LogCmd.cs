using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.WebApi.Instructions;

internal class LogCmd : INativeInstruction, INativeJSInstruction
{
  private readonly WebApplication app;

  internal LogCmd(WebApplication app)
  {
    this.app = app;
  }

  public string Key => Runtime.Constants.BasicInstructionsSet.Log;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => this.app.Logger.LogInformation(string.Join(" ", ctx.GetArgumentsValues().Select(m => m == null ? Runtime.Constants.Strings.Null : m.ToString())));
    }
  }

  public Delegate ToJSAction()
  {
    return new Action<string>(msg => Console.WriteLine(msg));
  }
}