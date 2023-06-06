using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime;

public class HtmlRuntime
{
  private Dictionary<string, Action<IRuntimeContext>> instructions = new Dictionary<string, Action<IRuntimeContext>>();

  private Dictionary<string, Delegate> jsInstructions = new Dictionary<string, Delegate>();

  private Stack<Context> ctxStack;

  private Context globalCtx;

  public HtmlRuntime()
  {
    this.ctxStack = new Stack<Context>();
    this.globalCtx = new Context(null, this.ctxStack);
  }

  public void Run(AppModel app)
  {
    //  JS Engine

    var jsParserWithContext = new JavascriptParserWithContext();

    foreach (var kv in this.jsInstructions)
    {
      jsParserWithContext.RegisterInstruction(kv.Key, kv.Value);
    }

    //  Title

    if (!string.IsNullOrEmpty(app.Title))
    {
      this.RunInstruction(Runtime.Constants.BasicInstructionsSet.SetTitle, app.Title);
    }

    //  Load functions

    foreach (var fn in app.Functions)
    {
      // Console.WriteLine("Function loaded" + fn.Id);
      jsParserWithContext.RegisterFunction($"function {fn.Id}({string.Join(",", fn.Arguments)}) {{ {fn.Code} }}");
    }

    //  Run instructions

    int cursor = 0;

    while (cursor < app.Instructions.Count)
    {
      try
      {
        var instruction = app.Instructions[cursor];

        var arguments = this.ParseArguments(jsParserWithContext, instruction.Arguments);

        this.RunInstruction(instruction.FunctionName, arguments);

        ++cursor;
      }
      catch (Jurassic.JavaScriptException ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }

  public void RunInstruction(string key, params string?[] args)
  {
    if (this.instructions.TryGetValue(key, out var action))
    {
      var ctx = this.globalCtx.Fork(key, args);

      action(ctx);
    }
    else
    {
      throw new InvalidOperationException($"Unknown instruction: {key}.");
    }
  }

  public void RegisterProvider(INativeProvider provider)
  {
    foreach (var instruction in provider.Instructions)
    {
      if (string.IsNullOrEmpty(provider.Namespace) || provider.Namespace == Runtime.Constants.Namespaces.Global)
      {
        this.instructions[instruction.Key] = instruction.Action;
        this.instructions[$"{Runtime.Constants.Namespaces.Global}.{instruction.Key}"] = instruction.Action;
      }
      else
      {
        this.instructions[$"{provider.Namespace}.{instruction.Key}"] = instruction.Action;
      }

      if (instruction is INativeJSInstruction)
      {
        var jsInstruction = (INativeJSInstruction)instruction;
        this.jsInstructions[instruction.Key] = jsInstruction.ToJSAction();
      }
    }
  }

  private string?[] ParseArguments(JavascriptParserWithContext jsParser, List<CallArgumentModel> arguments)
  {
    var result = new string?[arguments.Count];

    for (int i = 0; i < arguments.Count; i++)
    {
      if (arguments[i].IsCall)
      {
        if (string.IsNullOrEmpty(arguments[i].Content))
        {
          result[i] = null;
        }
        else
        {
          result[i] = jsParser.ExecuteCode(arguments[i].Content!)?.ToString();
        }
      }
      else if (arguments[i].IsSolve)
      {
        if (string.IsNullOrEmpty(arguments[i].Content))
        {
          result[i] = null;
        }
        else
        {
          result[i] = JavascriptParser.SimpleSolve(arguments[i].Content!)?.ToString();
        }
      }
      else
      {
        result[i] = arguments[i].Content;
      }
    }

    return result;
  }
}
