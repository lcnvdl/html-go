using System.Reflection;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.WebApi.Providers;

class ServerProvider : INativeProvider
{
  private readonly WebApplication app;

  internal ServerProvider(WebApplication app)
  {
    this.app = app;
  }

  public string Namespace => "Server";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new GetCmd(), };
}

class GetCmd : INativeInstruction
{
  public string Key => "ServerGet";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => throw new NotImplementedException();
    }
  }
}
