namespace HtmlRun.Runtime.Code;

public class JavascriptParserWithContext
{
  private Jurassic.ScriptEngine? engine;

  private Jurassic.ScriptEngine Engine
  {
    get
    {
      if (this.engine == null)
      {
        this.engine = new Jurassic.ScriptEngine();
        this.engine.SetGlobalValue("window", this.engine.Global);
      }

      return this.engine;
    }
  }

  public void RegisterInstruction(string name, Delegate code)
  {
    this.Engine.SetGlobalFunction(name, code);
  }

  public void RegisterFunction(string code)
  {
    this.Engine.Evaluate(code);
  }

  public object ExecuteCode(string code)
  {
    try
    {
      return this.Engine.Evaluate(code);
    }
    catch (Exception ex)
    {
      throw new Exception($"{ex.GetType().Name} error running the code {code}. {ex.Message}.");
    }
  }
}
