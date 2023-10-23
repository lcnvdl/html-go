using System.Reflection;
using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Tests.Stubs;

public abstract class BaseProviderTests
{
  private List<string> logs;

  private readonly Context ctx;

  private readonly HtmlRuntime runtime;

  protected Context Ctx => this.ctx;

  protected HtmlRuntime Runtime => this.runtime;

  private JavascriptParserWithContext? applicationJsContext;

  protected INativeProvider Provider { get; private set; }

  protected JavascriptParserWithContext JavascriptParserWithContext => this.applicationJsContext!;

  public BaseProviderTests(INativeProvider provider)
  {
    this.logs = new List<string>();
    this.Provider = provider;
    this.ctx = new Context(null, new Stack<Context>(), new Stack<GroupArguments>());
    this.runtime = new HtmlRuntime(this.ctx);
    this.runtime.RegisterProvider(new InstructionsProvider(this.logs));
    this.runtime.RegisterBasicProviders();

    var jurassicField = this.runtime.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).First(m => m.Name.Equals("applicationJsContext"));

    var newJsCtx = new JavascriptParserWithContext();

    jurassicField.SetValue(this.runtime, newJsCtx);

    this.applicationJsContext = (JavascriptParserWithContext)jurassicField.GetValue(this.runtime)!;

    if (this.applicationJsContext == null)
    {
      throw new NullReferenceException("applicationJsContext is null");
    }
  }

  protected void TestGetInstructions()
  {
    var result = this.Provider.Instructions;
    Assert.NotNull(result);
    Assert.NotEmpty(result);
  }

  protected INativeInstruction GetInstruction(string key)
  {
    return this.Provider.Instructions.First(m => m.Key.Equals(key));
  }

  protected INativeJSInstruction? GetJSInstruction(string key)
  {
    return this.Provider.Instructions.FirstOrDefault(m => m.Key.Equals(key)) as INativeJSInstruction;
  }

  protected object? CallJsInstruction(string key, params object?[] args)
  {
    var jsInstruction = this.GetJSInstruction(key);
    Assert.NotNull(jsInstruction);

    if (jsInstruction is IInstructionRequiresJsEngine engineReq)
    {
      engineReq.SetEngineGenerator(() => this.applicationJsContext!);
    }

    var jsAction = jsInstruction!.ToJSAction();

    object? result = jsAction.DynamicInvoke(args);

    return result;
  }
}