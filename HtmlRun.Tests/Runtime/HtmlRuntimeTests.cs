using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class HtmlRuntimeTests
{
  private HtmlRuntime runtime;

  public HtmlRuntimeTests()
  {
    this.runtime = new HtmlRuntime();
    this.runtime.RegisterProvider(new InstructionsProvider());
  }

  [Fact]
  public void HtmlRuntime_RegisterBasicProviders_ShouldWorkFine()
  {
    this.runtime.RegisterBasicProviders();
  }

  [Fact]
  public void HtmlRuntime_RunInstruction_ShouldWorkFine()
  {
    this.runtime.RunInstruction("Log", new ParsedArgument("this is a message", ParsedArgumentType.String));

    Assert.Single(LogCmd.Logs);
    Assert.Equal("this is a message", LogCmd.Logs[0]);
  }
}