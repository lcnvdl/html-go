using HtmlRun.Runtime;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.WebApi;

public static class Startup
{
  public static HtmlRuntime GetRuntime(WebApplication app)
  {
    var runtime = new HtmlRuntime();
    runtime.RegisterBasicProviders();
    runtime.RegisterProvider(new WebApiProvider(app));
    return runtime;
  }
}

class WebApiProvider : INativeProvider
{
  private readonly WebApplication app;

  internal WebApiProvider(WebApplication app)
  {
    this.app = app;
  }

  public string Namespace => Runtime.Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new LogCmd(app), new SetTitleCmd(), };
}

class SetTitleCmd : VoidInstruction
{
  public override string Key => Runtime.Constants.BasicInstructionsSet.SetTitle;
}

class LogCmd : INativeInstruction, INativeJSInstruction
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
      return ctx => this.app.Logger.LogInformation(string.Join(" ", ctx.GetArguments().Select(m => m == null ? Runtime.Constants.Strings.Null : m.ToString())));
    }
  }

  public Delegate ToJSAction()
  {
    return new Action<string>(msg => Console.WriteLine(msg));
  }
}