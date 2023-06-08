using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

public class HtmlRuntime
{
  private Dictionary<string, Action<ICurrentInstructionContext>> instructions = new Dictionary<string, Action<ICurrentInstructionContext>>();

  private Dictionary<string, Delegate> jsInstructions = new Dictionary<string, Delegate>();

  private Stack<Context> ctxStack;

  private Context globalCtx;

  public HtmlRuntime()
  {
    this.ctxStack = new Stack<Context>();
    this.globalCtx = new Context(null, this.ctxStack);
  }

  public void Run(AppModel app, CancellationToken? token)
  {
    //  JS Engine

    var jsParserWithContext = this.CreateNewJavascriptParserAndAssignInstructions();

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

    while (cursor < app.Instructions.Count && (!token.HasValue || !token.Value.IsCancellationRequested))
    {
      try
      {
        var instruction = app.Instructions[cursor];

        var arguments = this.ParseArguments(jsParserWithContext, instruction.Arguments);

        ICurrentInstructionContext finalCtx = this.RunInstruction(instruction.FunctionName, arguments);

        foreach (var variableKey in finalCtx.DirtyVariables)
        {
          var metaVariable = finalCtx.GetVariable(variableKey);

          if (metaVariable == null)
          {
            jsParserWithContext.ExecuteCode($"delete window.{variableKey}");
          }
          else
          {
            jsParserWithContext.ExecuteCode($"window.{variableKey}='{metaVariable.Value}'");
          }
        }

        if (finalCtx.CursorModification != null && !finalCtx.CursorModification.IsEmpty)
        {
          if (finalCtx.CursorModification is JumpToBranch)
          {
            var jump = (JumpToBranch)finalCtx.CursorModification;
            var newBranch = instruction.Arguments.Find(m => m.IsBranch && jump.IsBranch(m.BranchCondition));
            if (newBranch == null)
            {
              // if (jump.IsBranchRequired) // Bug 01
              // {
              throw new NullReferenceException($"Missing branch {jump.Condition} in {instruction.FunctionName} call.");
              // }
            }
            else
            {
              //  Causa del Bug 01 (me obliga a tener el case false en el IF, sino quedan instrucciones sueltas)
              app.Instructions.InsertRange(cursor + 1, newBranch.BranchInstructions!);
            }
          }
          else if (finalCtx.CursorModification is JumpToLine)
          {
            var jump = (JumpToLine)finalCtx.CursorModification;
            if (jump.JumpType == JumpToLine.JumpTypeEnum.LineNumber)
            {
              cursor = int.Parse(jump.Line!) - 1;
            }
            else
            {
              cursor = app.Instructions.FindIndex(m => m.CustomId != null && m.CustomId == jump.Line);
              if (cursor < 0)
              {
                throw new IndexOutOfRangeException();
              }

              --cursor;
            }
          }
          else
          {
            throw new InvalidCastException();
          }

          finalCtx.CursorModification = null;
        }

        ++cursor;
      }
      catch (Jurassic.JavaScriptException ex)
      {
        throw new Exception(ex.Message);
      }
    }

    if (token.HasValue && token.Value.IsCancellationRequested)
    {
      System.Diagnostics.Debug.WriteLine("Task Cancelled!");
    }
  }

  public ICurrentInstructionContext RunInstruction(string key, params string?[] args)
  {
    if (this.instructions.TryGetValue(key, out var action))
    {
      var ctx = this.globalCtx.Fork(key, args);

      action(ctx);

      return ctx;
    }
    else
    {
      throw new InvalidOperationException($"Unknown instruction: {key}.");
    }
  }

  public void RegisterBasicProviders()
  {
    //  Language instructions
    this.RegisterProvider(new VariablesProvider());
    this.RegisterProvider(new ConditionalProvider());
    this.RegisterProvider(new GotoProvider());

    //  Tools
    this.RegisterProvider(new DateProvider());

    //  TODO  Threading is not a basic provider
    this.RegisterProvider(new ThreadingProvider());
  }

  public void RegisterProvider(INativeProvider provider)
  {
    foreach (INativeInstruction instruction in provider.Instructions)
    {
      if (provider.IsGlobal)
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
        if (provider.IsGlobal)
        {
          this.jsInstructions[instruction.Key] = jsInstruction.ToJSAction();
        }
        else
        {
          this.jsInstructions[$"{provider.Namespace}.{instruction.Key}"] = jsInstruction.ToJSAction();
        }
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
      else if (arguments[i].IsPrimitive)
      {
        result[i] = arguments[i].Content;
      }
      else if (arguments[i].IsBranch)
      {
        result[i] = null;
      }
      else
      {
        throw new InvalidDataException($"Unknown argument type {arguments[i].ArgumentType}.");
      }
    }

    return result;
  }

  private JavascriptParserWithContext CreateNewJavascriptParserAndAssignInstructions()
  {
    var jsParserWithContext = new JavascriptParserWithContext();

    foreach (var kv in this.jsInstructions)
    {
      if (kv.Key.Contains("."))
      {
        string sanitizedKey = "call__" + kv.Key.Replace(".", "__");
        jsParserWithContext.RegisterInstruction(sanitizedKey, kv.Value);

        var split = kv.Key.Split('.');

        for (int i = 0; i < split.Length; i++)
        {
          string curr = split[i];

          for (int j = i - 1; j >= 0; j--)
          {
            curr = $"{split[j]}.{curr}";
          }

          jsParserWithContext.ExecuteCode($"window.{curr}=window.{curr}||{{}};");
        }

        jsParserWithContext.ExecuteCode($"window.{kv.Key}={sanitizedKey}");
      }
      else
      {
        jsParserWithContext.RegisterInstruction(kv.Key, kv.Value);
      }
    }

    return jsParserWithContext;
  }
}
