namespace HtmlRun.Runtime.Native;

public class JsActionDefinition
{
  public Delegate Delegate { get; set; }

  public JsActionDefinition(Delegate @delegate)
  {
    this.Delegate = @delegate;
  }
}