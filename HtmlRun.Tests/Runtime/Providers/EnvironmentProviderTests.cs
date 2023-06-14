using HtmlRun.Runtime;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;

public class EnvironmentProviderTests : BaseProviderTests
{
  public EnvironmentProviderTests() : base(new EnvironmentProvider())
  {
  }

  [Fact]
  public void EnvironmentProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void EnvironmentProvider_GetArgs_ShouldWorkFine()
  {
    var getArgs = this.GetJSInstruction(EnvironmentInstructionsSet.GetArgs);
    Assert.NotNull(getArgs);

    var jsAction = getArgs!.ToJSAction();

    var result = jsAction.DynamicInvoke();

    Assert.NotNull(result);
    Assert.IsAssignableFrom<string[]>(result);
  }

  [Fact]
  public void EnvironmentProvider_CurrentDirectory_ShouldWorkFine()
  {
    var getArgs = this.GetJSInstruction(EnvironmentInstructionsSet.CurrentDirectory);
    Assert.NotNull(getArgs);

    var jsAction = getArgs!.ToJSAction();

    var result = jsAction.DynamicInvoke();

    Assert.NotNull(result);
    Assert.IsAssignableFrom<string>(result);
    Assert.False(string.IsNullOrEmpty(result!.ToString()));
  }
}