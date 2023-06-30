using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;

namespace HtmlRun.Terminal;

static class Program
{
  async static Task Main(string[] args)
  {
    // Console.WriteLine(string.Join(", ", args));

    string? file = args.FirstOrDefault();

    Console.WriteLine();

    if (string.IsNullOrEmpty(file))
    {
      Console.WriteLine("Missing input file.");
      Environment.Exit(1);
      return;
    }

    if (file == "run" || file == "--run-example")
    {
      file = GetExample(args.Length > 1 ? int.Parse(args[1]) : 16);
      Console.WriteLine($"DEBUG MODE. Running example {file}...");
      Runtime.Utils.EnvironmentUtils.IsDevelopment = true;
    }

    if (!File.Exists(file))
    {
      Console.WriteLine($"File not found: {Path.GetFileName(file)}.");
      Environment.Exit(1);
      return;
    }

    if (!Path.GetExtension(file).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
    {
      Console.WriteLine($"Extension of \"{Path.GetFileName(file)}\" must be .html.");
      Environment.Exit(1);
      return;
    }

    await RunAppFromFile(file);
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

  private static string GetExample(int number)
  {
    return new DirectoryInfo(
      Directory.Exists(
        Path.Combine(Environment.CurrentDirectory, "../Examples")) ?
        Path.Combine(Environment.CurrentDirectory, "../Examples") :
        Path.Combine(Environment.CurrentDirectory, "./Examples")).GetFiles("*.html")[number].FullName;
  }
}
