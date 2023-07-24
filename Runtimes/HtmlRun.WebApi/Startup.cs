using HtmlRun.Runtime;
using HtmlRun.WebApi.Providers;

namespace HtmlRun.WebApi;

public static class Startup
{
  public static IHtmlRuntimeForApp GetRuntime(WebApplication app, WebApplicationBuilder builder)
  {
    IHtmlRuntimeForApp runtime = new HtmlRuntime();
    runtime.RegisterBasicProviders();
    runtime.RegisterProvider(new WebApiProvider(app));
    runtime.RegisterProvider(new ServerProvider(app, builder));
    return runtime;
  }
}
