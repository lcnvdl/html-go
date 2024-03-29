namespace HtmlRun.Runtime.RuntimeContext;

public class ContextValue
{
  private string? value;

  public string Name { get; set; }

  public virtual string? Value
  {
    get
    {
      return this.value;
    }

    set
    {
      if (!this.IsUnset && this.IsConst)
      {
        throw new InvalidOperationException();
      }

      if (this.IsUnset && value != null)
      {
        this.IsUnset = false;
      }

      this.value = value;
    }
  }

  public bool IsConst { get; set; }

  public virtual bool IsInferred => false;

  public bool IsUnset { get; private set; } = true;

  public bool IsExported { get; set; } = false;

  public ContextValue(string name)
  {
    this.Name = name;
    this.Value = null;
    this.IsConst = false;
  }

  public ContextValue(string name, string value, bool isConst)
  {
    this.Name = name;
    this.Value = value;
    this.IsConst = isConst;
  }
}