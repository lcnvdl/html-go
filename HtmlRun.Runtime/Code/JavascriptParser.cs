namespace HtmlRun.Runtime.Code;

public static class JavascriptParser
{
  public static object SimpleSolve(string code)
  {
    var engine = new Jurassic.ScriptEngine();
    var compilation = Jurassic.CompiledEval.Compile(new Jurassic.StringScriptSource(code));
    var result = compilation.Evaluate(engine);
    return result;
  }
}
