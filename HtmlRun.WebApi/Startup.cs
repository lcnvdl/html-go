using HtmlRun.Runtime;
using HtmlRun.WebApi.Providers;

namespace HtmlRun.WebApi;

public static class Startup
{
  public static HtmlRuntime GetRuntime(WebApplication app)
  {
    var runtime = new HtmlRuntime();
    runtime.RegisterBasicProviders();
    runtime.RegisterProvider(new WebApiProvider(app));
    runtime.RegisterProvider(new ServerProvider(app));
    return runtime;
  }
}
