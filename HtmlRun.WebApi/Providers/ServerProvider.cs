using System.Reflection;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using Microsoft.Extensions.FileProviders;

namespace HtmlRun.WebApi.Providers;

class ServerProvider : INativeProvider
{
  private readonly WebApplication app;

  private readonly WebApplicationBuilder builder;

  private readonly GlobalServerSettings settings;

  public string Namespace => "Server";

  internal ServerProvider(WebApplication app, WebApplicationBuilder builder)
  {
    this.app = app;
    this.builder = builder;
    this.settings = new GlobalServerSettings(app, builder);
  }

  public INativeInstruction[] Instructions => new INativeInstruction[]
  {
    new GetCmd(this.settings),
    new PostCmd(this.settings),
    new DeleteCmd(this.settings),
    new PutCmd(this.settings),
    new SetDefaultContentType(this.settings),
    new StaticFilesCmd(this.settings),
    new GetCurrentDirectory(this.settings),
    new StaticDirectoryCmd(this.settings),
  };
}

class GlobalServerSettings
{
  internal WebApplication App { get; private set; }

  internal WebApplicationBuilder Builder { get; private set; }

  internal string DefaultContentType { get; set; } = "text"; // json or text or html or stream (path) or (any mime type)

  internal string CurrentDirectory => this.Builder.Environment.ContentRootPath;

  public GlobalServerSettings(WebApplication app, WebApplicationBuilder builder)
  {
    this.App = app;
    this.Builder = builder;
  }
}

class GetCurrentDirectory : BaseCmdWithSettings, INativeInstruction, INativeJSInstruction
{
  public string Key => "GetCurrentDirectory";

  internal GetCurrentDirectory(GlobalServerSettings settings) : base(settings)
  {
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string>(() => this.Settings.CurrentDirectory);
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

class StaticFilesCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "StaticFiles";

  internal StaticFilesCmd(GlobalServerSettings settings) : base(settings)
  {
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var options = new FileServerOptions
        {
          FileProvider = new PhysicalFileProvider(this.ParseDirectory(ctx.GetRequiredArgument(1))),
          RequestPath = ctx.GetRequiredArgument(0),
          EnableDirectoryBrowsing = false
        };
        this.App.UseFileServer(options);
      };
    }
  }

  private string ParseDirectory(string argDir)
  {
    string dir = argDir;

    if (dir.StartsWith("./") || dir.StartsWith("../"))
    {
      dir = Path.Combine(this.Settings.CurrentDirectory, dir);
    }

    return dir;
  }
}

class StaticDirectoryCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "StaticDirectory";

  internal StaticDirectoryCmd(GlobalServerSettings settings) : base(settings)
  {
  }

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var options = new FileServerOptions
        {
          FileProvider = new PhysicalFileProvider(this.ParseDirectory(ctx.GetRequiredArgument(1))),
          RequestPath = ctx.GetRequiredArgument(0),
          EnableDirectoryBrowsing = true
        };
        this.App.UseFileServer(options);
      };
    }
  }

  private string ParseDirectory(string argDir)
  {
    string dir = argDir;

    if (dir.StartsWith("./") || dir.StartsWith("../"))
    {
      dir = Path.Combine(this.Settings.CurrentDirectory, dir);
    }

    return dir;
  }
}

class GetCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "Get";

  internal GetCmd(GlobalServerSettings settings) : base(settings) { }

  public Action<ICurrentInstructionContext> Action => (ctx => this.App.MapGet(this.GetPattern(ctx), this.GetDelegate(ctx)));
}

class PostCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "Post";

  internal PostCmd(GlobalServerSettings settings) : base(settings) { }

  public Action<ICurrentInstructionContext> Action => (ctx => this.App.MapPost(this.GetPattern(ctx), this.GetDelegate(ctx)));
}

class PutCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "Put";

  internal PutCmd(GlobalServerSettings settings) : base(settings) { }

  public Action<ICurrentInstructionContext> Action => (ctx => this.App.MapPut(this.GetPattern(ctx), this.GetDelegate(ctx)));
}

class DeleteCmd : BaseMethodCmd, INativeInstruction
{
  public string Key => "Delete";

  internal DeleteCmd(GlobalServerSettings settings) : base(settings) { }

  public Action<ICurrentInstructionContext> Action => (ctx => this.App.MapDelete(this.GetPattern(ctx), this.GetDelegate(ctx)));
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
