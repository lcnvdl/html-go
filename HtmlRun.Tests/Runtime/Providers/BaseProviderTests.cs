using HtmlRun.Runtime;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;

public abstract class BaseProviderTests
{
  protected INativeProvider Provider { get; private set; }

  public BaseProviderTests(INativeProvider provider)
  {
    this.Provider = provider;
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