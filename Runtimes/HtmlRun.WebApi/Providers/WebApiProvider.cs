using HtmlRun.Runtime.Native;
using HtmlRun.WebApi.Instructions;

namespace HtmlRun.WebApi.Providers;

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
