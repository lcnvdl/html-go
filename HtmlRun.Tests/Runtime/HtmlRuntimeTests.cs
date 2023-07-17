using HtmlRun.Common.Models;
using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class HtmlRuntimeTests
{
  private HtmlRuntime runtime;

  private List<string> logs;

  public HtmlRuntimeTests()
  {
    this.logs = new List<string>();

    this.runtime = new HtmlRuntime();
    this.runtime.RegisterProvider(new InstructionsProvider(this.logs));
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

    Assert.Single(this.logs);
    Assert.Equal("this is a message", this.logs[0]);
  }

  [Fact]
  public void HtmlRuntime_AppInfo_ShouldBeAvailable()
  {
    this.runtime.RegisterBasicProviders();

    var app = new AppModel();
    app.Version = "3.14.15";
    app.Title = "Test";
    app.Type = AppType.Console;
    app.InstructionGroups.Add(InstructionsGroup.Main);
    app.InstructionGroups[0].Instructions.Add("Log".AsCall(CallArgumentModel.FromCall("Application.Title")));
    app.InstructionGroups[0].Instructions.Add("Log".AsCall(CallArgumentModel.FromCall("Application.Version")));
    app.InstructionGroups[0].Instructions.Add("Log".AsCall(CallArgumentModel.FromCall("Application.Type")));

    this.runtime.Run(app, null);

    Assert.Equal(3, this.logs.Count);
    Assert.Equal("Test", this.logs[0]);
    Assert.Equal("3.14.15", this.logs[1]);
    Assert.Equal("Console", this.logs[2]);
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

    Assert.Single(this.logs);
    Assert.Equal("Lucho", this.logs[0]);
  }
}