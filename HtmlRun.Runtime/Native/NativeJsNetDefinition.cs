namespace HtmlRun.Runtime.Native;

public class NativeJSNetDefinition : INativeJSDefinition
{
  public Delegate Delegate { get; set; }

  public NativeJSNetDefinition(Delegate @delegate)
  {
    Delegate = @delegate;
  }
}