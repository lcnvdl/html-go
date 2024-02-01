using System.Text.Encodings.Web;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Interfaces;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
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

  private readonly Dictionary<string, HtmlRuntime> importedRuntimes = new();

  private readonly List<INativeProvider> registeredProviders = new();

  private readonly List<PluginBase> plugins = new();

  private readonly Stack<Context> ctxStack;

  private readonly Context globalCtx;

  private bool isApplicationStarted = false;

  private AppModel? application;

  private JavascriptParserWithContext? applicationJsContext;

  public HtmlRuntime(Context? globalCtx = null)
  {
    if (globalCtx == null)
    {
      this.ctxStack = new Stack<Context>();
      var argsStack = new Stack<GroupArguments>();
      this.globalCtx = new Context(null, this.ctxStack, argsStack);
    }
    else
    {
      this.globalCtx = globalCtx;
      this.ctxStack = globalCtx.CtxStack;
    }
  }

  public string[] Namespaces => this.instructions.Keys
    .Cast<string>()
    .Where(m => m.Contains('.'))
    .Select(m => new NamespaceModel(m, true).ToString())
    .ToArray();

  public void Run(AppModel app, CancellationToken? token)
  {
    //  Initialization

    this.Initialize(app);

    //  Run

    this.InternalRun(app, token);
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
    if (!this.globalCtx.IsDeclared(key, ignoreInferred: true))
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

    this.registeredProviders.Add(provider);
  }

  private void InternalRun(AppModel app, CancellationToken? token, IContextJump? initialCursorModification = null, StartApplicationAsFunctionModel? asFunctionModel = null)
  {
    //  Run instructions

    var applicationStartedModel = this.StartApplication(app, initialCursorModification, asFunctionModel);

    var cursor = applicationStartedModel.NewCursor;
    var appInstructions = applicationStartedModel.AppInstructions;

    this.RunImportedLibraries(app, token);

    while (cursor.Position < appInstructions.Count && (!token.HasValue || !token.Value.IsCancellationRequested))
    {
      this.DoStep(cursor, appInstructions);
    }

    if (token.HasValue && token.Value.IsCancellationRequested)
    {
      System.Diagnostics.Debug.WriteLine("Task Cancelled!");
    }
  }

  private void Initialize(AppModel app)
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

    if (!string.IsNullOrEmpty(app.Title) && app.Type != AppType.Library)
    {
      this.RunInstruction(BasicInstructionsSet.SetTitle, new ParsedArgument(app.Title, ParsedArgumentType.String));
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

    //  Prepare imported runtimes

    foreach (var import in app.Imports)
    {
      var newRuntime = NewHtmlRuntimeWithSameDependencies();
      this.importedRuntimes[import.Library.Id] = newRuntime;

      import.Library.Alias = import.Alias;

      newRuntime.Initialize(import.Library);
    }
  }

  private HtmlRuntime NewHtmlRuntimeWithSameDependencies()
  {
    var runtime = new HtmlRuntime();

    foreach (var provider in this.registeredProviders)
    {
      runtime.RegisterProvider(provider);
    }

    return runtime;
  }

  private ApplicationStartedModel StartApplication(AppModel app, IContextJump? initialCursorModification, StartApplicationAsFunctionModel? startAsFunction)
  {
    this.isApplicationStarted = true;

    var cursor = new InstructionPointer(app.Id, startAsFunction?.CallStack);

    InstructionsGroup? main = app.InstructionGroups.SingleOrDefault(m => m.Label == InstructionsGroup.MainLabel);

    if (main == null)
    {
      if (app.Type == AppType.Library)
      {
        main = InstructionsGroup.Main;
      }
      else
      {
        throw new InvalidOperationException("Main instructions group not found.");
      }
    }

    List<CallModel> appInstructions = main.Instructions;

    this.ctxStack.Clear();
    this.ctxStack.Push(this.globalCtx);

    if (startAsFunction != null)
    {
      if (startAsFunction.ArgsAndValues != null)
      {
        this.globalCtx.InitialPushArgumentsAndValues(startAsFunction.ArgsAndValues);
      }
    }

    var model = new ApplicationStartedModel(cursor, main, appInstructions);

    if (initialCursorModification != null)
    {
      cursor.ApplyJumpOrFail(initialCursorModification, appInstructions);
    }

    return model;
  }

  private void DoStep(InstructionPointer cursor, List<CallModel> appInstructions)
  {
    try
    {
      CallModel instruction = appInstructions[cursor.Position];

      ParsedArgument[] arguments = this.ParseArguments(this.applicationJsContext!, instruction.Arguments);

      ICurrentInstructionContext finalCtx = this.RunInstruction(this.ctxStack.Peek(), instruction.FunctionName, arguments);

      finalCtx.SaveContextVariableChangesToJsEngine(this.applicationJsContext!);

      this.ApplyJumpAndContextSwitching(finalCtx, cursor, appInstructions);

      cursor.MoveToNextPosition();

      //  External application run
      if (!cursor.IsPointingToSameApplication(this.application))
      {
        this.RunExternalFunction(finalCtx, cursor, instruction);
      }
    }
    catch (Jurassic.JavaScriptException ex)
    {
      throw new Exception(ex.Message);
    }
  }

  private void RunExternalFunction(ICurrentInstructionContext finalCtx, InstructionPointer cursor, CallModel instruction)
  {
    //  TODO  Set cancellation token
    if (finalCtx.CursorModification == null)
    {
      throw new NullReferenceException($"Cursor modification is null for instruction {instruction.FunctionName}.");
    }

    var currentCursorModification = finalCtx.CursorModification as JumpToLine;

    if (currentCursorModification == null)
    {
      throw new NullReferenceException($"Cursor modification is null for instruction {instruction.FunctionName}.");
    }

    if (currentCursorModification.Line == null)
    {
      throw new NullReferenceException($"Cursor modification line is null for instruction {instruction.FunctionName}.");
    }

    var cleanJump = new JumpToLine(currentCursorModification.Line, currentCursorModification.JumpType, currentCursorModification.Offset + 1);
    // var cleanJump = new JumpToLineWithCallStack(currentCursorModification.Line, currentCursorModification.JumpType, currentCursorModification.Offset + 1);

    GroupArguments? argsAndValues = finalCtx.PopArgumentsAndValues();
    var asFunctionModel = new StartApplicationAsFunctionModel
    {
      ArgsAndValues = argsAndValues,
      CallStack = cursor.CallStack,
    };

    var importedRuntime = this.importedRuntimes[cursor.ApplicationId];
    var app = importedRuntime.application!;

    importedRuntime.InternalRun(app, null, cleanJump, asFunctionModel);

    finalCtx.CursorModification = null;

    //  TODO  Should restore cursor and application ID by using a return. The next line should not be released:
    cursor.UnsafeRecoverFromApplicationContextChange();

    if (this.application != null && cursor.ApplicationId != this.application.Id)
    {
      throw new InvalidOperationException("Application ID is not the same after external application run.");
    }

    //  Get exported variables
    foreach (ContextValue variable in importedRuntime.globalCtx.AllVariables.Where(m => m != null))
    {
      string groupPrefix = (argsAndValues == null || string.IsNullOrEmpty(argsAndValues.Label)) ? string.Empty : $"{argsAndValues.Label}.";

      this.ImportVariablesFromRuntime(importedRuntime, groupPrefix);
    }
  }

  private void ImportVariablesFromRuntime(HtmlRuntime child, string groupPrefix = "")
  {
    var app = child.application!;

    //  Get exported variables
    foreach (ContextValue variable in child.globalCtx.AllVariables.Where(m => m != null))
    {
      string? libraryAlias = app?.Alias;

      string libraryAliasPrefix = string.IsNullOrEmpty(libraryAlias) ? string.Empty : $"{libraryAlias}.";

      string finalName = $"{libraryAliasPrefix}{groupPrefix}{variable.Name}";

      this.globalCtx.AddVariable(new ContextValue(finalName, variable.Value!, variable.IsConst));

      this.applicationJsContext?.SafeSet(finalName, variable.Value);
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
        Context? ctxToDispose = this.ctxStack.Pop();

        if (this.ctxStack.Count == 0)
        {
          throw new InvalidOperationException("Context stack is not balanced.");
        }

        ctxToDispose?.Release();
      }
      else if (finalCtx.CursorModification is IExternalJumpWithMemory)
      {
        List<CallModel> appInstructionsForJump = this.GetAppInstructionsFromExternalJump(finalCtx.CursorModification);

        cursor.ApplyJumpOrFail(finalCtx.CursorModification, appInstructionsForJump);

        //  Should keep the cursor modification
        // finalCtx.CursorModification = null;
      }
      else
      {
        cursor.ApplyJumpOrFail(finalCtx.CursorModification, appInstructions);

        finalCtx.CursorModification = null;
      }
    }
  }

  private List<CallModel> GetAppInstructionsFromExternalJump(IContextJump contextJump)
  {
    var jumpAsJumpToLine = (JumpToLine)contextJump;

    var jumpAsExternalJump = (IExternalJumpWithMemory)contextJump;

    var application = this.importedRuntimes[jumpAsExternalJump.ApplicationId].application! ??
      throw new NullReferenceException($"Application is null in {jumpAsExternalJump.ApplicationId}.");

    return application.InstructionGroups.First(group => group.Instructions.Any(i => i.CustomId == jumpAsJumpToLine.Line)).Instructions;
  }

  private ICurrentInstructionContext RunInstruction(IRuntimeContext parentCtx, string key, params ParsedArgument[] args)
  {
    var action = this.GetActionOrFail(parentCtx, key, args);

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

  private Action<ICurrentInstructionContext> GetActionOrFail(IRuntimeContext ctx, string key, ParsedArgument[] args)
  {
    if (this.instructions.ContainsKey(key))
    {
      return this.instructions[key];
    }

    if (this.application == null)
    {
      throw new NullReferenceException("Application is null.");
    }

    if (this.application.LabeledGroups.Any())
    {
      var group = this.application.LabeledGroups.FirstOrDefault(m => m.Label.Equals(key));
      if (group != null)
      {
        string groupStartLabel = CompilerConstants.GroupStartLabel(this.application!, group);

        return ctx =>
        {
          ctx.CursorModification = new JumpToLineWithCallStack(groupStartLabel, JumpToLine.JumpTypeEnum.LineId);

          if (group.HasArguments)
          {
            ctx.PushArgumentsAndValues(GroupArguments.GetPartialWithValuesFromGroup(group, args));
          }
        };
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
        string aliasPrefix = string.IsNullOrEmpty(import.Alias) ? "" : $"{import.Alias}.";
        var group = import.Library.InstructionGroups.Find(m => $"{aliasPrefix}{m.Label}" == key);

        if (group != null)
        {
          //  Debería hacer un jump no solo  a la linea, sino a la aplicacion en sí
          // return ctx => ctx.CursorModification = new JumpToLineWithCallStack($"import-{import.Path}-{group.Label}", JumpToLine.JumpTypeEnum.LineId, 0, );

          // throw new NotImplementedException("Imported libraries are not implemented yet.");

          string groupStartLabel = CompilerConstants.GroupStartLabel(import.Library, group);

          return ctx =>
          {
            ctx.CursorModification = new JumpToExternalLineWithCallStack(import.Library.Id, groupStartLabel, JumpToLine.JumpTypeEnum.LineId);
            ctx.PushArgumentsAndValues(group.HasArguments ?
              GroupArguments.GetPartialWithValuesFromGroup(group, args) :
              GroupArguments.Empty(group.Label));
          };
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

          ctx.SetValueVariable(resultKey, jsResult);
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

  private void RunImportedLibraries(AppModel app, CancellationToken? token)
  {
    var imports = app.Imports.FindAll(m => m.Library.InstructionGroups.Any(m => m.IsMain && m.Instructions.Count > 0));

    if (imports.Count == 0)
    {
      return;
    }

    var runtimes = imports.Select(m => new { runtime = this.importedRuntimes[m.Library.Id], app = m.Library });

    Parallel.ForEach(runtimes, m => m.runtime.InternalRun(m.app, token));

    foreach (var import in imports)
    {
      var runtime = this.importedRuntimes[import.Library.Id];
      this.ImportVariablesFromRuntime(runtime);
    }
  }
}
