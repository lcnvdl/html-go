using HtmlRun.Runtime;

namespace HtmlRun.Terminal;

public static class Startup
{
  public static HtmlRuntime GetRuntime()
  {
    var runtime = new HtmlRuntime();
    runtime.RegisterProvider(new TerminalProvider());
    return runtime;
  }
}