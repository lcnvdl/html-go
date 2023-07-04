using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;

namespace HtmlRun.Terminal;

static class Program
{
  async static Task Main(string[] args)
  {
    ProgramArgs? argsModel = null;

    try
    {
      argsModel = ProgramArgsProcessor.ProcessInputAndGetModel(args, 0);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      Environment.Exit(1);
    }

    if (argsModel.ShowVersionAndFinish)
    {
      var assembly = typeof(Program).Assembly;
      string version = assembly.GetName().Version?.ToString() ?? "0.0.0.0";
      Console.WriteLine($"v{version}");
      return;
    }

    await RunAppFromFile(argsModel.File);
  }

  private static async Task RunAppFromFile(string file)
  {
    var runtime = Startup.GetRuntime();

    Environment.SetEnvironmentVariable("ENTRY_FILE", file);
    Environment.SetEnvironmentVariable("ENTRY_DIRECTORY", Path.GetDirectoryName(file));

    var appModel = await ReadApp(file);

    runtime.Run(appModel, null);
  }

  private static async Task<AppModel> ReadApp(string file)
  {
    IInterpreter spider = new SpiderInterpreter();

    AppModel app = await spider.ParseString(File.ReadAllText(file));

    return app;
  }
}
