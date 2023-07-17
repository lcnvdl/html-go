namespace HtmlRun.Runtime.Native;

public interface INativeJSEvalInstruction : INativeJSBaseInstruction
{
  EvalDefinition ToEvalFunction();
}

public class EvalDefinition
{
  public string Function { get; set; }

  public int Arguments { get; set; }

  public EvalDefinition(string function, int arguments)
  {
    this.Function = function;
    this.Arguments = arguments;
  }
}