namespace HtmlRun.Runtime.Native;

public class NativeJSEvalDefinition : INativeJSDefinition
{
  public int Arguments { get; set; }

  public string Function { get; set; }

  public NativeJSEvalDefinition(EvalDefinition evalDefinition)
  {
    this.Function = evalDefinition.Function;
    this.Arguments = evalDefinition.Arguments;
  }
}