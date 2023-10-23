using HtmlRun.Runtime.Code;

namespace HtmlRun.Runtime.Native;

public class NativeJSEvalDefinition : INativeJSDefinition
{
  public int Arguments { get; set; }

  public string Function { get; set; }

  public Func<JavascriptParserWithContext> JsEngineGetter { get; set; }

  public NativeJSEvalDefinition(EvalDefinition evalDefinition, Func<JavascriptParserWithContext> jsEngineGetter)
  {
    this.Function = evalDefinition.Function;
    this.Arguments = evalDefinition.Arguments;
    this.JsEngineGetter = jsEngineGetter;
  }
}