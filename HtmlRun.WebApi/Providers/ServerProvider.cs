using System.Reflection;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.WebApi.Providers;

class ServerProvider : INativeProvider
{
  private readonly WebApplication app;

  public string Namespace => "Server";

  internal ServerProvider(WebApplication app)
  {
    this.app = app;
  }

  public INativeInstruction[] Instructions => new INativeInstruction[] { new GetCmd(this.app), };
}

class GetCmd : INativeInstruction
{
  private readonly WebApplication app;

  public string Key => "Get";

  internal GetCmd(WebApplication app)
  {
    this.app = app;
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => 
      {
        string arg0 = ctx.GetRequiredArgument();
        string arg1 = ctx.GetRequiredArgument(1);
        ParsedArgument arg1Metadata = ctx.GetArgumentAt(1);

        IUnsafeCurrentInstructionContext unsafeCtx = (IUnsafeCurrentInstructionContext)ctx;
        this.app.MapGet(arg0, arg1Metadata.IsReference ? () => unsafeCtx.Runtime.RunCallReference(ctx, arg1) : () => arg1);
      };
    }
  }
}
