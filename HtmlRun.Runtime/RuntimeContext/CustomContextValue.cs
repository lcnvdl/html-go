namespace HtmlRun.Runtime.RuntimeContext;

public sealed class CustomContextValue : ContextValue
{
  public object CustomValue { get; set; }

  public CustomContextValue(string name, string textValue, object value, bool isConst)
  : base(name, textValue, isConst)
  {
    this.CustomValue = value;
  }
}