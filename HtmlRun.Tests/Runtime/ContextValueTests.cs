using HtmlRun.Runtime;
using HtmlRun.Runtime.RuntimeContext;

public class ContextValueTests
{
  [Fact]
  public void ContextValue_Ctor_ShouldWorkFine()
  {
    var value = new ContextValue("name");
    Assert.Equal("name", value.Name);
    Assert.False(value.IsConst);
    Assert.True(value.IsUnset);
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
}