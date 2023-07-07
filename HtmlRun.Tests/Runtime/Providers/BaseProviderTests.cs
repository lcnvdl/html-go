using HtmlRun.Runtime;
using HtmlRun.Runtime.Native;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public abstract class BaseProviderTests
{
  protected INativeProvider Provider { get; private set; }

  private readonly Context ctx;

  private readonly HtmlRuntime runtime;

  protected Context Ctx => this.ctx;

  protected HtmlRuntime Runtime => this.runtime;

  public BaseProviderTests(INativeProvider provider)
  {
    this.Provider = provider;
    this.ctx = new Context(null, new Stack<Context>());
    this.runtime = new HtmlRuntime();
    this.runtime.RegisterProvider(new InstructionsProvider());
    this.runtime.RegisterBasicProviders();
    LogCmd.Logs.Clear();
  }

  protected void TestGetInstructions()
  {
    var result = this.Provider.Instructions;
    Assert.NotNull(result);
    Assert.NotEmpty(result);
  }

  protected INativeInstruction GetInstruction(string key)
  {
    return this.Provider.Instructions.First(m => m.Key.Equals(key));
  }

  protected INativeJSInstruction? GetJSInstruction(string key)
  {
    return this.Provider.Instructions.FirstOrDefault(m => m.Key.Equals(key)) as INativeJSInstruction;
  }

  protected object? CallJsInstruction(string key, params object?[] args)
  {
    var jsInstruction = this.GetJSInstruction(key);
    Assert.NotNull(jsInstruction);

    var jsAction = jsInstruction!.ToJSAction();

    object? result = jsAction.DynamicInvoke(args);

    return result;
  }
}