using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;
using HtmlRun.Runtime;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

[Collection("IntegrationTests")]
public class HtmlRuntimeExamplesTests : IDisposable
{
  private HtmlRuntime runtime;
  private SpiderInterpreter spider;

  public HtmlRuntimeExamplesTests()
  {
    LogCmd.Logs.Clear();

    this.runtime = new HtmlRuntime();
    this.runtime.RegisterBasicProviders();
    this.runtime.RegisterProvider(new InstructionsProvider());

    this.spider = new SpiderInterpreter();
  }

  public void Dispose()
  {
    LogCmd.Logs.Clear();
  }

  [Fact]
  public async void HtmlRuntime_Example_1()
  {
    var app = await this.ReadApp("01-hello_world");

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("Hello world", LogCmd.Logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_2()
  {
    var app = await this.ReadApp("02-simple_operation");

    Assert.Equal("1.0.1", app.Version);
    Assert.Equal(AppType.Console, app.Type);

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("The result of 1 + 2 is 3", LogCmd.Logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_3()
  {
    var app = await this.ReadApp("03-custom_calls");

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("The result of Fibonacci(13) is 233", LogCmd.Logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_4()
  {
    var app = await this.ReadApp("04-conditionals");

    this.runtime.Run(app, null);

    Assert.Equal(2, LogCmd.Logs.Count);
    Assert.Equal("3 > 2... Yep", LogCmd.Logs[0]);
    Assert.Equal("1 > 2... Nope", LogCmd.Logs[1]);
  }

  [Fact]
  public async void HtmlRuntime_Example_6()
  {
    var app = await this.ReadApp("06-variables");

    this.runtime.Run(app, null);

    Assert.Equal(5, LogCmd.Logs.Count);
    Assert.Equal("Language: HyperText Machine Language", LogCmd.Logs[0]);
    Assert.Equal("You have a new message: Hi there!", LogCmd.Logs[1]);
    Assert.Equal("Reply: Hello", LogCmd.Logs[2]);
    Assert.Contains("Current time: ", LogCmd.Logs[3]);
    Assert.Contains("Next second value: ", LogCmd.Logs[4]);
  }

  [Fact]
  public async void HtmlRuntime_Example_7()
  {
    var app = await this.ReadApp("07-variables_thread_safe");

    this.runtime.Run(app, null);

    // Assert.Single(LogCmd.Logs);
    Assert.Equal(3, LogCmd.Logs.Count);
    Assert.Equal("Value: 0", LogCmd.Logs[0]);
    Assert.Equal("Value (+1): 1", LogCmd.Logs[1]);
    Assert.Equal("Value (-1): 0", LogCmd.Logs[2]);
  }

  [Fact]
  public async void HtmlRuntime_Example_8()
  {
    var app = await this.ReadApp("08-while_using_goto_if");

    this.runtime.Run(app, null);

    Assert.Equal(6, LogCmd.Logs.Count);
    Assert.Equal("Current iteration: 0", LogCmd.Logs[0]);
    Assert.Equal("Current iteration: 1", LogCmd.Logs[1]);
    Assert.Equal("Current iteration: 2", LogCmd.Logs[2]);
    Assert.Equal("Current iteration: 3", LogCmd.Logs[3]);
    Assert.Equal("Current iteration: 4", LogCmd.Logs[4]);
    Assert.Equal("Exit", LogCmd.Logs[5]);
  }

  [Fact]
  public async void HtmlRuntime_Example_9()
  {
    var app = await this.ReadApp("09-using");

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("Sleep was called without the namespace (Threading.Sleep).", LogCmd.Logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_12()
  {
    var app = await this.ReadApp("12-conditionals_II");

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("3 > 2... Yep", LogCmd.Logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_13()
  {
    var app = await this.ReadApp("13-conditionals_switch");

    this.runtime.Run(app, null);

    Assert.Equal(3, LogCmd.Logs.Count);
    Assert.Equal("Arf arf!", LogCmd.Logs[0]);
    Assert.Equal("Squeek!", LogCmd.Logs[1]);
    Assert.Equal("Moo!", LogCmd.Logs[2]);
  }

  [Fact]
  public async void HtmlRuntime_Example_14()
  {
    var app = await this.ReadApp("14-while");

    this.runtime.Run(app, null);

    Assert.Equal(5, LogCmd.Logs.Count);
    Assert.Equal("Iteration: 0", LogCmd.Logs[0]);
    Assert.Equal("Iteration: 1", LogCmd.Logs[1]);
    Assert.Equal("Iteration: 2", LogCmd.Logs[2]);
    Assert.Equal("Iteration: 3", LogCmd.Logs[3]);
    Assert.Equal("Iteration: 4", LogCmd.Logs[4]);
  }

  [Fact]
  public async void HtmlRuntime_Example_15()
  {
    var app = await this.ReadApp("15-do_while");

    this.runtime.Run(app, null);

    Assert.Equal("Do-While is working fine when condition is false", LogCmd.Logs[0]);
    Assert.Equal("Iteration: 0", LogCmd.Logs[1]);
    Assert.Equal("Iteration: 1", LogCmd.Logs[2]);
    Assert.Equal("Iteration: 2", LogCmd.Logs[3]);
    Assert.Equal("Iteration: 3", LogCmd.Logs[4]);
    Assert.Equal("Iteration: 4", LogCmd.Logs[5]);
    Assert.Equal(6, LogCmd.Logs.Count);
  }

  [Fact]
  public async void HtmlRuntime_Example_16()
  {
    var app = await this.ReadApp("16-for");

    this.runtime.Run(app, null);

    Assert.Equal(5, LogCmd.Logs.Count);
    Assert.Equal("Iteration: 0", LogCmd.Logs[0]);
    Assert.Equal("Iteration: 1", LogCmd.Logs[1]);
    Assert.Equal("Iteration: 2", LogCmd.Logs[2]);
    Assert.Equal("Iteration: 3", LogCmd.Logs[3]);
    Assert.Equal("Iteration: 4", LogCmd.Logs[4]);
  }

  [Fact]
  public async void HtmlRuntime_Example_17()
  {
    var app = await this.ReadApp("17-nested_loops");

    this.runtime.Run(app, null);

    Assert.Equal(16 * 16, LogCmd.Logs.Count);
  }

  //  Partial comment. Will be implemented in another PR.
  /*[Fact]
  public async void HtmlRuntime_Example_18()
  {
    var app = await this.ReadApp("18-library_import");

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("H", LogCmd.Logs[0]);
  }*/

  [Fact]
  public async void HtmlRuntime_Example_19()
  {
    var app = await this.ReadApp("19-call_stack");

    this.runtime.Run(app, null);

    Assert.Equal(4, LogCmd.Logs.Count);
    Assert.Equal("Begin", LogCmd.Logs[0]);
    Assert.Equal("Inside the function", LogCmd.Logs[1]);
    Assert.Equal("Closing app...", LogCmd.Logs[2]);
    Assert.Equal("Exit", LogCmd.Logs[3]);
  }


  [Fact]
  public async void HtmlRuntime_Example_20()
  {
    var app = await this.ReadApp("20-instruction_groups");

    this.runtime.Run(app, null);

    Assert.Equal(3, LogCmd.Logs.Count);
    Assert.Equal("Program started", LogCmd.Logs[0]);
    Assert.Equal("Info", LogCmd.Logs[1]);
    Assert.Equal("Program finished", LogCmd.Logs[2]);
  }

  private async Task<AppModel> ReadApp(string exampleName)
  {
    string file = Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.IndexOf("HtmlRun.Tests"));
    var app = await this.spider.ParseString($"{exampleName}.html", m => File.ReadAllText($"{file}/Examples/{m}"));
    Assert.NotNull(app);
    return app!;
  }
}