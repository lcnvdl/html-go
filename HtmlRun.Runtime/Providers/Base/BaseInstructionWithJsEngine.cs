using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

internal abstract class BaseInstructionWithJsEngine : IInstructionRequiresJsEngine
{
  private Func<JavascriptParserWithContext>? engineGetter;

  public void SetEngineGenerator(Func<object> engineGetter)
  {
    this.engineGetter = () => (JavascriptParserWithContext)engineGetter();
  }

  protected JavascriptParserWithContext Engine => this.engineGetter!();

  protected Jurassic.ScriptEngine Jurassic => this.engineGetter!().Engine;
}

