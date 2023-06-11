using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;
using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class HtmlRuntimeExamplesTests: IDisposable
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
  public async void HtmlRuntime_Example_9()
  {
    var app = await this.ReadApp("09-using");

    this.runtime.Run(app, null);

    Assert.Single(LogCmd.Logs);
    Assert.Equal("Sleep was called without the namespace (Threading.Sleep).", LogCmd.Logs[0]);
  }

  private async Task<AppModel> ReadApp(string exampleName)
  {
    string file = Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.IndexOf("HtmlRun.Tests"));
    var app = await this.spider.ParseString(File.ReadAllText($"{file}/Examples/{exampleName}.html"));
    Assert.NotNull(app);
    return app!;
  }
}