using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;
using HtmlRun.Runtime;
using HtmlRun.Tests.Stubs;

public class HtmlRuntimeExamplesTests
{
  private HtmlRuntime runtime;

  private SpiderInterpreter spider;

  private List<string> logs;

  public HtmlRuntimeExamplesTests()
  {
    this.logs = new List<string>();

    this.runtime = new HtmlRuntime();
    this.runtime.RegisterBasicProviders();
    this.runtime.RegisterProvider(new InstructionsProvider(this.logs));

    this.spider = new SpiderInterpreter();
  }

  [Fact]
  public async void HtmlRuntime_Example_1()
  {
    var app = await this.ReadApp("01-hello_world");

    this.runtime.Run(app, null);

    Assert.Single(this.logs);
    Assert.Equal("Hello world", this.logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_2()
  {
    var app = await this.ReadApp("02-simple_operation");

    Assert.Equal("1.0.1", app.Version);
    Assert.Equal(AppType.Console, app.Type);

    this.runtime.Run(app, null);

    Assert.Single(this.logs);
    Assert.Equal("The result of 1 + 2 is 3", this.logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_3()
  {
    var app = await this.ReadApp("03-custom_calls");

    this.runtime.Run(app, null);

    Assert.Single(this.logs);
    Assert.Equal("The result of Fibonacci(13) is 233", this.logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_4()
  {
    var app = await this.ReadApp("04-conditionals");

    this.runtime.Run(app, null);

    Assert.Equal(2, this.logs.Count);
    Assert.Equal("3 > 2... Yep", this.logs[0]);
    Assert.Equal("1 > 2... Nope", this.logs[1]);
  }

  [Fact]
  public async void HtmlRuntime_Example_6()
  {
    var app = await this.ReadApp("06-variables");

    this.runtime.Run(app, null);

    Assert.Equal(5, this.logs.Count);
    Assert.Equal("Language: HyperText Machine Language", this.logs[0]);
    Assert.Equal("You have a new message: Hi there!", this.logs[1]);
    Assert.Equal("Reply: Hello", this.logs[2]);
    Assert.Contains("Current time: ", this.logs[3]);
    Assert.Contains("Next second value: ", this.logs[4]);
  }

  [Fact]
  public async void HtmlRuntime_Example_7()
  {
    var app = await this.ReadApp("07-variables_thread_safe");

    this.runtime.Run(app, null);

    // Assert.Single(this.logs);
    Assert.Equal(3, this.logs.Count);
    Assert.Equal("Value: 0", this.logs[0]);
    Assert.Equal("Value (+1): 1", this.logs[1]);
    Assert.Equal("Value (-1): 0", this.logs[2]);
  }

  [Fact]
  public async void HtmlRuntime_Example_8()
  {
    var app = await this.ReadApp("08-while_using_goto_if");

    this.runtime.Run(app, null);

    Assert.Equal(6, this.logs.Count);
    Assert.Equal("Current iteration: 0", this.logs[0]);
    Assert.Equal("Current iteration: 1", this.logs[1]);
    Assert.Equal("Current iteration: 2", this.logs[2]);
    Assert.Equal("Current iteration: 3", this.logs[3]);
    Assert.Equal("Current iteration: 4", this.logs[4]);
    Assert.Equal("Exit", this.logs[5]);
  }

  [Fact]
  public async void HtmlRuntime_Example_9()
  {
    var app = await this.ReadApp("09-using");

    this.runtime.Run(app, null);

    Assert.Single(this.logs);
    Assert.Equal("Sleep was called without the namespace (Threading.Sleep).", this.logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_12()
  {
    var app = await this.ReadApp("12-conditionals_II");

    this.runtime.Run(app, null);

    Assert.Single(this.logs);
    Assert.Equal("3 > 2... Yep", this.logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_13()
  {
    var app = await this.ReadApp("13-conditionals_switch");

    this.runtime.Run(app, null);

    Assert.Equal(3, this.logs.Count);
    Assert.Equal("Arf arf!", this.logs[0]);
    Assert.Equal("Squeek!", this.logs[1]);
    Assert.Equal("Moo!", this.logs[2]);
  }

  [Fact]
  public async void HtmlRuntime_Example_14()
  {
    var app = await this.ReadApp("14-while");

    this.runtime.Run(app, null);

    Assert.Equal(5, this.logs.Count);
    Assert.Equal("Iteration: 0", this.logs[0]);
    Assert.Equal("Iteration: 1", this.logs[1]);
    Assert.Equal("Iteration: 2", this.logs[2]);
    Assert.Equal("Iteration: 3", this.logs[3]);
    Assert.Equal("Iteration: 4", this.logs[4]);
  }

  [Fact]
  public async void HtmlRuntime_Example_15()
  {
    var app = await this.ReadApp("15-do_while");

    this.runtime.Run(app, null);

    Assert.Equal("Do-While is working fine when condition is false", this.logs[0]);
    Assert.Equal("Iteration: 0", this.logs[1]);
    Assert.Equal("Iteration: 1", this.logs[2]);
    Assert.Equal("Iteration: 2", this.logs[3]);
    Assert.Equal("Iteration: 3", this.logs[4]);
    Assert.Equal("Iteration: 4", this.logs[5]);
    Assert.Equal(6, this.logs.Count);
  }

  [Fact]
  public async void HtmlRuntime_Example_16()
  {
    var app = await this.ReadApp("16-for");

    this.runtime.Run(app, null);

    Assert.Equal(5, this.logs.Count);
    Assert.Equal("Iteration: 0", this.logs[0]);
    Assert.Equal("Iteration: 1", this.logs[1]);
    Assert.Equal("Iteration: 2", this.logs[2]);
    Assert.Equal("Iteration: 3", this.logs[3]);
    Assert.Equal("Iteration: 4", this.logs[4]);
  }

  [Fact]
  public async void HtmlRuntime_Example_17()
  {
    var app = await this.ReadApp("17-nested_loops");

    this.runtime.Run(app, null);

    Assert.Equal(16 * 16, this.logs.Count);
  }

  [Fact]
  public async void HtmlRuntime_Example_18()
  {
    var app = await this.ReadApp("18-library_import");

    this.runtime.Run(app, null);

    Assert.Single(this.logs);
    Assert.Equal("[20230802-19:00:00] Hello world", this.logs[0]);
  }

  [Fact]
  public async void HtmlRuntime_Example_19()
  {
    var app = await this.ReadApp("19-call_stack");

    this.runtime.Run(app, null);

    Assert.Equal(4, this.logs.Count);
    Assert.Equal("Begin", this.logs[0]);
    Assert.Equal("Inside the function", this.logs[1]);
    Assert.Equal("Closing app...", this.logs[2]);
    Assert.Equal("Exit", this.logs[3]);
  }

  [Fact]
  public async void HtmlRuntime_Example_20()
  {
    var app = await this.ReadApp("20-instruction_groups");

    this.runtime.Run(app, null);

    Assert.Equal(3, this.logs.Count);
    Assert.Equal("Program started", this.logs[0]);
    Assert.Equal("[Info] Test log", this.logs[1]);
    Assert.Equal("Program finished", this.logs[2]);
  }

  [Fact]
  public async void HtmlRuntime_Example_21()
  {
    var app = await this.ReadApp("21-reference_values");

    this.runtime.Run(app, null);

    Assert.Equal(3, this.logs.Count);
    Assert.Equal("Current ID: 1", this.logs[0]);
    Assert.Equal("Current Name: HtmlGo!", this.logs[1]);
    Assert.Equal("Next ID: 2", this.logs[2]);
  }

  [Fact]
  public async void HtmlRuntime_Example_23()
  {
    var app = await this.ReadApp("23-lists");

    this.runtime.Run(app, null);

    Assert.Equal(6, this.logs.Count);
    Assert.Equal("World", this.logs[0]);
    Assert.Equal("Hello", this.logs[1]);
    Assert.Equal("Hello", this.logs[2]);
    Assert.Equal("World", this.logs[3]);
    Assert.Equal("World", this.logs[4]);
    Assert.Equal("0", this.logs[5]);
  }

  [Fact]
  public async void HtmlRuntime_Example_24()
  {
    var app = await this.ReadApp("24-strings");

    this.runtime.Run(app, null);

    Assert.Equal(3, this.logs.Count);
    Assert.Equal("Line 1: Hi", this.logs[0]);
    Assert.Equal("Line 2: world!", this.logs[1]);
    Assert.Equal("Hi--world!", this.logs[2]);
  }

  private async Task<AppModel> ReadApp(string exampleName)
  {
    string file = Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.IndexOf("HtmlRun.Tests"));
    var app = await this.spider.ParseString($"{exampleName}.html", m => File.ReadAllText($"{file}/Examples/{m}"));
    Assert.NotNull(app);
    return app!;
  }
}