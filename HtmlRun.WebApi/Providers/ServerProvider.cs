using System.Reflection;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.WebApi.Providers;

class ServerProvider : INativeProvider
{
  private readonly WebApplication app;

  private readonly GlobalServerSettings settings;

  public string Namespace => "Server";

  internal ServerProvider(WebApplication app)
  {
    this.app = app;
    this.settings = new GlobalServerSettings(app);
  }

  public INativeInstruction[] Instructions => new INativeInstruction[] { new GetCmd(this.settings), new SetDefaultContentType(this.settings), };
}

class GlobalServerSettings
{
  internal WebApplication App { get; private set; }

  internal string DefaultContentType { get; set; } = "text"; // json or text or html or stream (path) or (any mime type)

  public GlobalServerSettings(WebApplication app)
  {
    this.App = app;
  }
}

class SetDefaultContentType : BaseCmdWithSettings, INativeInstruction
{
  public string Key => "SetDefaultContentType";

  internal SetDefaultContentType(GlobalServerSettings settings) : base(settings)
  {
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => this.Settings.DefaultContentType = ctx.GetRequiredArgument();
    }
  }
}

class GetCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "Get";

  internal GetCmd(GlobalServerSettings settings) : base(settings)
  {
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        this.App.MapGet(this.GetPattern(ctx), this.GetDelegate(ctx));
      };
    }
  }
}

abstract class BaseMethodCmd : BaseCmdWithSettings
{
  internal BaseMethodCmd(GlobalServerSettings settings) : base(settings)
  {
  }

  protected string GetPattern(ICurrentInstructionContext ctx)
  {
    string arg0 = ctx.GetRequiredArgument();
    return arg0;
  }

  protected Delegate GetDelegate(ICurrentInstructionContext ctx)
  {
    string arg1 = ctx.GetRequiredArgument(1);
    ParsedArgument arg1Metadata = ctx.GetArgumentAt(1);

    string contentType = ctx.GetArgument(2) ?? this.Settings.DefaultContentType;
    bool isStream = false;

    switch (contentType)
    {
      case "json":
        contentType = "application/json";
        break;
      case "text":
        contentType = "text/plain";
        break;
      case "html":
        contentType = "text/html";
        break;
      case "file":
      case "stream":
        isStream = true;
        break;
    }

    IUnsafeCurrentInstructionContext unsafeCtx = (IUnsafeCurrentInstructionContext)ctx;
    return arg1Metadata.IsReference ?
      (() => isStream ?
        Results.Stream(new FileStream(unsafeCtx.Runtime.RunCallReference(ctx, arg1) ?? string.Empty, FileMode.Open)) :
        Results.Text(unsafeCtx.Runtime.RunCallReference(ctx, arg1) ?? string.Empty, contentType)) :
      (() => isStream ?
        Results.Stream(new FileStream(arg1, FileMode.Open), fileDownloadName: Path.GetFileName(arg1)) :
        Results.Text(arg1, contentType));
  }
}

abstract class BaseCmdWithSettings
{
  private readonly GlobalServerSettings settings;

  protected GlobalServerSettings Settings => this.settings;

  protected WebApplication App => this.settings.App;

  internal BaseCmdWithSettings(GlobalServerSettings settings)
  {
    this.settings = settings;
  }
}
