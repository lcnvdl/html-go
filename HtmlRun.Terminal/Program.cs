var spider = new HtmlRun.Interpreter.Interpreters.SpiderInterpreter();

// Console.WriteLine(string.Join(",", Environment.GetCommandLineArgs()));

string? file = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();

Console.WriteLine();

if (string.IsNullOrEmpty(file))
{
  Console.WriteLine("Missing input file.");
  Environment.Exit(1);
  return;
}

if (file == "run")
{
  Console.WriteLine("DEBUG MODE. Running example...");
  // file = Path.Combine(Environment.CurrentDirectory, "../Examples/01-hello_world.html");
  // file = Path.Combine(Environment.CurrentDirectory, "../Examples/02-simple_operation.html");
  // file = Path.Combine(Environment.CurrentDirectory, "../Examples/03-custom_calls.html");
  file = Path.Combine(Environment.CurrentDirectory, "../Examples/04-conditionals.html");
}

if (!File.Exists(file))
{
  Console.WriteLine($"File not found: {file}.");
  Environment.Exit(1);
  return;
}

if (!Path.GetExtension(file).Equals(".html", StringComparison.InvariantCultureIgnoreCase))
{
  Console.WriteLine($"Extension of \"{Path.GetFileName(file)}\" must be .html.");
  Environment.Exit(1);
  return;
}

var app = await spider.ParseString(File.ReadAllText(file));

var runtime = HtmlRun.Terminal.Startup.GetRuntime();

// runtime.RunInstruction("Log", new string[] { "Hola", "mundo!" });

runtime.Run(app);

Console.WriteLine();

// Console.WriteLine("App result");
// Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(app));

// Console.WriteLine(app?.ToString() ?? HtmlRun.Runtime.Constants.Strings.Null);