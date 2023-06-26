using HtmlRun.Common.Models;
using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class HtmlRuntimeTests : IDisposable
{
  private HtmlRuntime runtime;

  public HtmlRuntimeTests()
  {
    this.runtime = new HtmlRuntime();
    this.runtime.RegisterProvider(new InstructionsProvider());
    LogCmd.Logs.Clear();
  }

  public void Dispose()
  {
    LogCmd.Logs.Clear();
  }

  [Fact]
  public void HtmlRuntime_RegisterBasicProviders_ShouldWorkFine()
  {
    this.runtime.RegisterBasicProviders();
  }

  [Fact]
  public void HtmlRuntime_RunInstruction_ShouldWorkFine()
  {
    this.runtime.RunInstruction("Log", ParsedArgument.String("this is a message"));

    Assert.Single(LogCmd.Logs);
    Assert.Equal("this is a message", LogCmd.Logs[0]);
  }

  [Fact]
  public void HtmlRuntime_ShouldHaveVariablesSynchronizedWithJsEngine()
  {
    this.runtime.RegisterBasicProviders();

    var app = new AppModel();
    app.InstructionGroups.Add(InstructionsGroup.Main);
    app.InstructionGroups[0].Instructions.Add("Var".AsCall(CallArgumentModel.FromString("name")));
    app.InstructionGroups[0].Instructions.Add("Set".AsCall(CallArgumentModel.FromString("name"), CallArgumentModel.FromString("Lucho")));
    app.InstructionGroups[0].Instructions.Add("Log".AsCall(CallArgumentModel.FromCall("name")));

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("Lucho", LogCmd.Logs[0]);
  }
}