namespace HtmlRun.Terminal;

static class ProgramArgsProcessor
{
  public static ProgramArgs ProcessInputAndGetModel(string[] args, int defaultExample)
  {
    var model = new ProgramArgs();

    string? file = args.FirstOrDefault();

    if (string.IsNullOrEmpty(file))
    {
      throw new FileNotFoundException("Missing input file.");
    }

    if (file == "-v" || file == "--version")
    {
      model.ShowVersionAndFinish = true;
      return model;
    }

    if (file == "run" || file == "--run-example")
    {
      file = GetExample(args.Length > 1 ? int.Parse(args[1]) : defaultExample);
      Console.WriteLine($"DEBUG MODE. Running example {file}...");
      Runtime.Utils.EnvironmentUtils.IsDevelopment = true;
    }

    if (!File.Exists(file))
    {
      throw new FileNotFoundException($"File not found: {Path.GetFileName(file)}.");
    }

    if (!Path.GetExtension(file).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
    {
      throw new FileLoadException($"Extension of \"{Path.GetFileName(file)}\" must be .html.");
    }

    model.File = file!;

    return model;
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