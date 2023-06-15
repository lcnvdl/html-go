using HtmlRun.Runtime;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public abstract class BaseProviderTests
{
  protected INativeProvider Provider { get; private set; }

  private Context ctx;

  private HtmlRuntime runtime;

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

  [Fact]
  public void ContextValue_CtorAlt_ShouldWorkFine()
  {
    var value = new ContextValue("animal", "dog", true);
    Assert.Equal("animal", value.Name);
    Assert.Equal("dog", value.Value);
    Assert.True(value.IsConst);
    Assert.False(value.IsUnset);
  }

  [Fact]
  public void Context_SetValueToAssignedConst_ShouldFail()
  {
    var pi = new ContextValue("pi", "3.14", true);
    Assert.Throws<InvalidOperationException>(() => pi.Value = "3.141");
  }

  protected INativeInstruction GetInstruction(string key)
  {
    return this.Provider.Instructions.First(m => m.Key.Equals(key));
  }

  protected INativeJSInstruction? GetJSInstruction(string key)
  {
    return this.Provider.Instructions.FirstOrDefault(m => m.Key.Equals(key)) as INativeJSInstruction;
  }
}