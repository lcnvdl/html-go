using HtmlRun.Common.Models;

namespace HtmlRun.WebApi;

static class Program
{
  async static Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    //  HtmlGo

    var runtime = HtmlRun.WebApi.Startup.GetRuntime(app, builder);

    string? file = args.FirstOrDefault();

    if (string.IsNullOrEmpty(file))
    {
      app.Logger.LogError("Missing input file.");
      Environment.Exit(1);
      return;
    }

    if (file == "run" || file == "--run-example")
    {
      file = GetExample(args.Length > 1 ? int.Parse(args[1]) : 0);
      app.Logger.LogInformation("DEBUG MODE. Running example...");
    }

    if (!File.Exists(file))
    {
      app.Logger.LogError($"File not found: {Path.GetFileName(file)}.");
      Environment.Exit(1);
      return;
    }

    if (!Path.GetExtension(file).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
    {
      app.Logger.LogError($"Extension of \"{Path.GetFileName(file)}\" must be .html.");
      Environment.Exit(1);
      return;
    }

    AppModel appModel = await ReadAppFromFile(file);

    Environment.SetEnvironmentVariable("ENTRY_FILE", file);
    Environment.SetEnvironmentVariable("ENTRY_DIRECTORY", Path.GetDirectoryName(file));

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || file == "run" || args.Contains("--swagger"))
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // app.UseAuthorization();
    // app.MapControllers();

    runtime.Run(appModel, null);

    // app.MapGet("/", () => "Works");

    app.Run();
  }

  private static async Task<AppModel> ReadAppFromFile(string file)
  {
    var spider = new HtmlRun.Interpreter.Interpreters.SpiderInterpreter();

    AppModel app = await spider.ParseString(File.ReadAllText(file));

    return app;
  }

  private static string GetExample(int number)
  {
    return new DirectoryInfo(
      Directory.Exists(
        Path.Combine(Environment.CurrentDirectory, "../Examples/WebApi")) ?
        Path.Combine(Environment.CurrentDirectory, "../Examples/WebApi") :
        Path.Combine(Environment.CurrentDirectory, "./Examples/WebApi")).GetFiles("*.html")[number].FullName;
  }
}

