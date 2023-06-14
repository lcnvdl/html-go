using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Models;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Runtime.Utils;

namespace HtmlRun.Runtime;

public class HtmlRuntime : IHtmlRuntimeForContext
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

  public string[] Namespaces => this.instructions.Keys
    .Cast<string>()
    .Where(m => m.Contains('.'))
    .Select(m => new NamespaceModel(m, true).ToString())
    .ToArray();

  public void Run(AppModel app, CancellationToken? token)
  {
    //  JS Engine

    var jsParserWithContext = JavascriptParserWithContextFactory.CreateNewJavascriptParserAndAssignInstructions(this.jsInstructions);

    //  Title

    if (!string.IsNullOrEmpty(app.Title))
    {
      this.RunInstruction(Runtime.Constants.BasicInstructionsSet.SetTitle, new ParsedArgument(app.Title, ParsedArgumentType.String));
    }

    //  Load functions

    foreach (var fn in app.Functions)
    {
      // Console.WriteLine("Function loaded" + fn.Id);
      jsParserWithContext.RegisterFunction($"function {fn.Id}({string.Join(",", fn.Arguments)}) {{ {fn.Code} }}");
    }

    //  Run instructions

    int cursor = 0;

    this.ctxStack.Clear();
    this.ctxStack.Push(this.globalCtx);

    while (cursor < app.Instructions.Count && (!token.HasValue || !token.Value.IsCancellationRequested))
    {
      try
      {
        CallModel instruction = app.Instructions[cursor];

        ParsedArgument[] arguments = this.ParseArguments(jsParserWithContext, instruction.Arguments);

        ICurrentInstructionContext finalCtx = this.RunInstruction(this.ctxStack.Peek(), instruction.FunctionName, arguments);

        this.SaveContextVariableChangesToJsEngine(finalCtx, jsParserWithContext);

        if (finalCtx.CursorModification != null && !finalCtx.CursorModification.IsEmpty)
        {
          cursor = this.GetNewCursorPositionAfterJump(finalCtx, instruction, cursor, app);
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

  public string? RunCallReference(ICurrentInstructionContext ctx, string referenceId)
  {
    this.instructions[referenceId].Invoke(ctx);
    var variable = ctx.GetVariable(referenceId);
    if (variable == null)
    {
      throw new NotImplementedException();
    }

    var result = variable.Value;

    ctx.DeleteVariable(referenceId);

    return result;
  }

  public ICurrentInstructionContext RunInstruction(string key, params ParsedArgument[] args)
  {
    return this.RunInstruction(this.globalCtx, key, args);
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

  private ICurrentInstructionContext RunInstruction(IRuntimeContext parentCtx, string key, params ParsedArgument[] args)
  {
    var action = this.GetActionOrFail(parentCtx, key);

    var instructionCtx = parentCtx.Fork(this, key, args);

    action(instructionCtx);

    return instructionCtx;
  }

  private int GetNewCursorPositionAfterJump(ICurrentInstructionContext finalCtx, CallModel instruction, int cursor, AppModel app)
  {
    //  TODO  AppModel app is temporary. Should not be there.

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
        //  TODO Causa del Bug 01 (me obliga a tener el case false en el IF, sino quedan instrucciones sueltas)
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

    return cursor;
  }

  private Action<ICurrentInstructionContext> GetActionOrFail(IRuntimeContext ctx, string key)
  {
    if (this.instructions.ContainsKey(key))
    {
      return this.instructions[key];
    }

    if (ctx.Usings.Any())
    {
      //  TODO  Optimization: Precompute context instructions.
      //  TODO  Bug: Instructions must be saved in context, otherwise "call" and "callReference" params will fail.

      foreach (var usng in ctx.Usings)
      {
        string usingDot = $"{usng}.";

        foreach (var kv in this.instructions)
        {
          if (kv.Key.StartsWith(usingDot) && kv.Key.EndsWith(key))
          {
            string shortKey = kv.Key.Substring(usingDot.Length);
            if (shortKey == key)
            {
              return kv.Value;
            }
          }
        }
      }
    }

    throw new InvalidOperationException($"Unknown instruction: {key}.");
  }

  private ParsedArgument[] ParseArguments(JavascriptParserWithContext jsParser, List<CallArgumentModel> arguments)
  {
    var result = new ParsedArgument[arguments.Count];

    for (int i = 0; i < arguments.Count; i++)
    {
      result[i] = ParsedArgumentFactory.CreateFromCallArgumentModel(arguments[i], jsParser);

      if (arguments[i].IsCallReference && !string.IsNullOrEmpty(arguments[i].Content))
      {
        string code = arguments[i].Content!;
        string key = result[i].Value!;

        this.instructions[key] = ctx =>
        {
          string? jsResult = jsParser.ExecuteCode(code)?.ToString();
          string resultKey = key;

          if (!ctx.ExistsVariable(resultKey))
          {
            ctx.DeclareVariable(resultKey);
          }

          ctx.SetVariable(resultKey, jsResult);
        };

        result[i] = new ParsedArgument(key, ParsedArgumentType.Reference);
      }
    }

    return result;
  }

  private void SaveContextVariableChangesToJsEngine(ICurrentInstructionContext finalCtx, JavascriptParserWithContext jsParserWithContext)
  {
    foreach (string variableKey in finalCtx.DirtyVariables)
    {
      ContextValue? metaVariable = finalCtx.GetVariable(variableKey);

      if (metaVariable == null)
      {
        jsParserWithContext.ExecuteCode($"delete window.{variableKey}");
      }
      else
      {
        jsParserWithContext.ExecuteCode($"window.{variableKey}='{metaVariable.Value}'");
      }
    }
  }
}
