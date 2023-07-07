using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;

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
  public void EnvironmentProvider_SetEnvironmentVariable_ShouldWorkFine()
  {
    var instruction = this.GetInstruction(EnvironmentInstructionsSet.SetEnvironmentVariable);
    Assert.NotNull(instruction);

    var action = instruction!.Action;

    action.Invoke(this.Ctx.Fork(this.Runtime, instruction.Key, new[] { ParsedArgument.String("NewVar"), ParsedArgument.String("qwerty") }));

    Assert.Equal("qwerty", Environment.GetEnvironmentVariable("NewVar"));
  }

  [Fact]
  public void EnvironmentProvider_SetEnvironmentVariableJS_ShouldWorkFine()
  {
    this.CallJsInstruction(EnvironmentInstructionsSet.SetEnvironmentVariable, "NewJSVar", "qwerty");

    Assert.Equal("qwerty", Environment.GetEnvironmentVariable("NewJSVar"));
  }

  [Fact]
  public void EnvironmentProvider_GetEnvironmentVariable_ShouldWorkFine()
  {
    var getEnvVar = this.GetJSInstruction(EnvironmentInstructionsSet.GetEnvironmentVariable);
    Assert.NotNull(getEnvVar);

    var jsAction = getEnvVar!.ToJSAction();

    Environment.SetEnvironmentVariable("NewEnvVar", "abc");

    var result = jsAction.DynamicInvoke("NewEnvVar");

    Assert.NotNull(result);
    Assert.IsAssignableFrom<string>(result);
    Assert.Equal("abc", result!.ToString());

    var resultForNonExistingVars = jsAction.DynamicInvoke("NewEnvVarB");

    Assert.Null(resultForNonExistingVars);
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