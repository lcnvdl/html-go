namespace HtmlRun.WebApi;

static class ProgramArgsProcessor
{
  public static ProgramArgs ProcessInputAndGetModel(string[] args, int defaultExample, ILogger logger)
  {
    var model = new ProgramArgs();

    string? file = args.FirstOrDefault();

    if (string.IsNullOrEmpty(file))
    {
      logger.LogError("Missing input file.");
      Environment.Exit(1);
    }

    if (file == "-v" || file == "--version")
    {
      model.ShowVersionAndFinish = true;
      return model;
    }

    if (file == "run" || file == "--run-example")
    {
      file = GetExample(args.Length > 1 ? int.Parse(args[1]) : defaultExample);
      logger.LogInformation("DEBUG MODE. Running example...");
      Runtime.Utils.EnvironmentUtils.IsDevelopment = true;
    }

    if (!File.Exists(file))
    {
      logger.LogError($"File not found: {Path.GetFileName(file)}.");
      Environment.Exit(1);
    }

    if (!Path.GetExtension(file).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
    {
      logger.LogError($"Extension of \"{Path.GetFileName(file)}\" must be .html.");
      Environment.Exit(1);
    }

    model.File = file;
    model.UseSwagger = file == "run" || args.Contains("--swagger");
    model.Https = args.Contains("--https");

    return model;
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