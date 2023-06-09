using HtmlRun.Runtime;

namespace HtmlRun.Terminal;

public static class Startup
{
  public static HtmlRuntime GetRuntime()
  {
    var runtime = new HtmlRuntime();
    runtime.RegisterBasicProviders();
    runtime.RegisterProvider(new GlobalProvider());
    runtime.RegisterProvider(new TerminalProvider());
    return runtime;
  }
}