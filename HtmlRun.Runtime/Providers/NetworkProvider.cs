using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.Utils;

namespace HtmlRun.Runtime.Providers;

public class HttpClientProvider : INativeProvider
{
  public string Namespace => "Network";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new HttpGetCmd(),
  };
}

static class HttpStaticModule
{
  private static HttpClient httpClient = new HttpClient();

  internal static HttpClient HttpClient => httpClient;
}

class HttpGetCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.NetworkInstructionsSet.HttpGet;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(url =>
    {
      string result = AsyncUtils.ToSync(() => HttpStaticModule.HttpClient.GetStringAsync(url));
      return result;
    });
  }
}
