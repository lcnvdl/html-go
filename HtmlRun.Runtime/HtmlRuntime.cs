using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Factories;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Models;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Runtime.Utils;

namespace HtmlRun.Runtime;

public class HtmlRuntime : IHtmlRuntimeForApp, IHtmlRuntimeForContext, IHtmlRuntimeForUnsafeContext
{
  private readonly Dictionary<string, Action<ICurrentInstructionContext>> instructions = new();

  private readonly Dictionary<string, INativeJSDefinition> jsInstructions = new();

  private readonly Stack<Context> ctxStack = new();

  private readonly List<PluginBase> plugins = new();

  private readonly Context globalCtx;

  private bool isApplicationStarted = false;

  private AppModel? application;

  private JavascriptParserWithContext? applicationJsContext;

  public HtmlRuntime()
  {
    this.globalCtx = new Context(null, this.ctxStack);
  }

  public string[] Namespaces => this.instructions.Keys
    .Cast<string>()
    .Where(m => m.Contains('.'))
    .Select(m => new NamespaceModel(m, true).ToString())
    .ToArray();

  public void Run(AppModel app, CancellationToken? token)
  {
    //  Application

    if (this.application != null)
    {
      throw new InvalidOperationException("There was an application already loaded for this runtime.");
    }

    this.application = app;

    //  Plugins: IBeforeApplicationLoadPluginEvent

    if (this.plugins.Count > 0)
    {
      var startInfo = new ApplicationStartEventModel(Environment.GetEnvironmentVariables().ToDictionary<string, string>());
      this.TriggerPlugins<IBeforeApplicationLoadPluginEvent>(plugin => plugin.BeforeApplicationLoad(startInfo));
    }

    //  JS Engine

    var jsParserWithContext = JavascriptParserWithContextFactory.CreateNewJavascriptParserAndAssignInstructions(this.jsInstructions);

    this.applicationJsContext = jsParserWithContext;

    //  App info
    this.globalCtx.DeclareAndSetConst("Application.Title", application.Title);
    this.globalCtx.DeclareAndSetConst("Application.Version", application.Version);
    this.globalCtx.DeclareAndSetConst("Application.Type", application.Type.ToString());

    jsParserWithContext.ExecuteCode($"window.Application = {{ Title: '{application.Title}', Version: '{application.Version}', Type: '{application.Type}', }}");

    //  Title

    if (!string.IsNullOrEmpty(app.Title))
    {
      this.RunInstruction(Constants.BasicInstructionsSet.SetTitle, new ParsedArgument(app.Title, ParsedArgumentType.String));
    }

    //  Load functions

    foreach (var fn in app.Functions)
    {
      jsParserWithContext.RegisterFunction($"function {fn.Id}({string.Join(",", fn.Arguments)}) {{ {fn.Code} }}");
    }

    //  Load entities

    foreach (EntityModel entity in app.Entities)
    {
      this.globalCtx.DeclareAndSetConst($"Entities.{entity.Name}", System.Text.Json.JsonSerializer.Serialize(entity));
      this.TriggerPlugins<IOnLoadEntityPluginEvent>(plugin => plugin.OnLoadEntity(entity));
    }

    //  Plugins: IOnApplicationStartPluginEvent

    if (this.plugins.Count > 0)
    {
      var startInfo = new ApplicationStartEventModel(Environment.GetEnvironmentVariables().ToDictionary<string, string>());
      this.TriggerPlugins<IOnApplicationStartPluginEvent>(plugin => plugin.OnApplicationStart(startInfo));
    }

    //  Compile instructions

    HtmlRuntimeCompiler.CompileInstructions(app);

    //  Run instructions

    this.isApplicationStarted = true;

    var cursor = new InstructionPointer();

    InstructionsGroup? main = app.InstructionGroups.SingleOrDefault(m => m.Label == InstructionsGroup.MainLabel);
    if (main == null)
    {
      throw new InvalidOperationException("Main instructions group not found.");
    }

    List<CallModel> appInstructions = main.Instructions;

    this.ctxStack.Clear();
    this.ctxStack.Push(this.globalCtx);

    while (cursor.Position < appInstructions.Count && (!token.HasValue || !token.Value.IsCancellationRequested))
    {
      try
      {
        CallModel instruction = appInstructions[cursor.Position];

        ParsedArgument[] arguments = this.ParseArguments(jsParserWithContext, instruction.Arguments);

        ICurrentInstructionContext finalCtx = this.RunInstruction(this.ctxStack.Peek(), instruction.FunctionName, arguments);

        this.SaveContextVariableChangesToJsEngine(finalCtx, jsParserWithContext);

        this.ApplyJumpAndContextSwitching(finalCtx, cursor, appInstructions);

        cursor.MoveToNextPosition();
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

  public void RegisterPlugin(PluginBase plugin)
  {
    this.plugins.Add(plugin);
    plugin.Providers?.ToList().ForEach(this.RegisterProvider);

    //  For information
    string key = $"Plugin.{plugin.Name}";
    if (!this.globalCtx.IsDeclared(key))
    {
      this.globalCtx.DeclareAndSetConst(key, "true");
    }

    //  Trigger events if app was already running
    if (this.isApplicationStarted)
    {
      //  TODO  Optimization: assign only new instructions
      if (this.applicationJsContext != null)
      {
        JavascriptParserWithContextFactory.AssignInstructions(this.applicationJsContext, this.jsInstructions);
      }

      if (plugin is IBeforeApplicationLoadPluginEvent beforeLoadListener)
      {
        var startInfo = new ApplicationStartEventModel(Environment.GetEnvironmentVariables().ToDictionary<string, string>());
        beforeLoadListener.BeforeApplicationLoad(startInfo);
      }

      if (plugin is IOnApplicationStartPluginEvent onStartListener)
      {
        var startInfo = new ApplicationStartEventModel(Environment.GetEnvironmentVariables().ToDictionary<string, string>());
        onStartListener.OnApplicationStart(startInfo);
      }

      if (plugin is IOnLoadEntityPluginEvent entityListener)
      {
        this.application?.Entities.ForEach(entityListener.OnLoadEntity);
      }
    }
  }

  public void RegisterProvider(INativeProvider provider)
  {
    foreach (INativeInstruction instruction in provider.Instructions)
    {
      //  Instructions
      if (provider.IsGlobal)
      {
        this.instructions[instruction.Key] = instruction.Action;
        this.instructions[$"{Constants.Namespaces.Global}.{instruction.Key}"] = instruction.Action;
      }
      else
      {
        this.instructions[$"{provider.Namespace}.{instruction.Key}"] = instruction.Action;
      }

      //  JS Engine Instructions
      if (instruction is INativeJSBaseInstruction jsInstruction)
      {
        string definitionName = provider.IsGlobal ? instruction.Key : $"{provider.Namespace}.{instruction.Key}";

        this.jsInstructions[definitionName] = NativeJSDefinitionFactory.NewInstance(jsInstruction);
      }
    }
  }

  private void ApplyJumpAndContextSwitching(ICurrentInstructionContext finalCtx, InstructionPointer cursor, List<CallModel> appInstructions)
  {
    if (finalCtx.CursorModification != null && !finalCtx.CursorModification.IsEmpty)
    {
      if (finalCtx.CursorModification is ContextPush)
      {
        var newCtx = this.ctxStack.Peek().Fork();
        this.ctxStack.Push(newCtx);
      }
      else if (finalCtx.CursorModification is ContextPull)
      {
        this.ctxStack.Pop();

        if (this.ctxStack.Count == 0)
        {
          throw new InvalidOperationException("Context stack is not balanced.");
        }
      }
      else
      {
        cursor.ApplyJumpOrFail(finalCtx.CursorModification, appInstructions);
        finalCtx.CursorModification = null;
      }
    }
  }

  private ICurrentInstructionContext RunInstruction(IRuntimeContext parentCtx, string key, params ParsedArgument[] args)
  {
    var action = this.GetActionOrFail(parentCtx, key);

    var instructionCtx = parentCtx.Fork(this, key, args);

    try
    {
      action(instructionCtx);
    }
    catch (Exception ex)
    {
      if (EnvironmentUtils.IsDevelopment)
      {
        string argsDetails = string.Join(", ", args.Select(m => m.Value));
        throw new Exception($"{ex.GetType().Name} error at call {key}({argsDetails}). {ex.Message}.", ex);
      }

      throw;
    }

    return instructionCtx;
  }

  private Action<ICurrentInstructionContext> GetActionOrFail(IRuntimeContext ctx, string key)
  {
    if (this.instructions.ContainsKey(key))
    {
      return this.instructions[key];
    }

    if (this.application.LabeledGroups.Any())
    {
      foreach (var group in this.application.LabeledGroups)
      {
        if (group.Label.Equals(key))
        {
          return ctx => ctx.CursorModification = new JumpToLineWithCallStack($"group-{group.Label}", JumpToLine.JumpTypeEnum.LineId);
        }
      }
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

    if (this.application!.Imports.Any())
    {
      //  TODO  Optimization: Precompute context instructions.
      //  TODO  Bug: Instructions must be saved in context, otherwise "call" and "callReference" params will fail.

      foreach (var import in this.application.Imports)
      {
        string prefix = string.IsNullOrEmpty(import.Alias) ? "" : $"{import.Alias}.";
        var group = import.Library.InstructionGroups.Find(m => $"{prefix}{m.Label}" == key);

        if (group != null)
        {
          throw new NotImplementedException("Imported libraries are not implemented yet.");
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

      //  Extra parsing if it is a "callReference". TODO: Refactor this.

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

  private void TriggerPlugins<T>(Action<T> action)
  {
    this.plugins.Where(plugin => plugin is T).Cast<T>().ToList().ForEach(action);
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
