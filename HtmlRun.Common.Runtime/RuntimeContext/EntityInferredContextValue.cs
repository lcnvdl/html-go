namespace HtmlRun.Runtime.RuntimeContext;

public class DictionaryInferredContextValue : ContextValue
{
  private readonly string attributeName;

  private readonly Dictionary<string, object?> reference;

  public override bool IsInferred => true;

  public override string? Value
  {
    get
    {
      base.Value = this.GetValueOrFail()?.ToString();

      return base.Value;
    }

    set
    {
      base.Value = value;

      if (this.reference != null)
      {
        this.reference[this.attributeName] = value;
      }
    }
  }

  public DictionaryInferredContextValue(Dictionary<string, object?> reference, string name) : base(name)
  {
    this.reference = reference;
    this.attributeName = name;
  }

  private object? GetValueOrFail()
  {
    if (string.IsNullOrEmpty(attributeName) || !this.reference.ContainsKey(attributeName))
    {
      throw new Exception($"Field {attributeName} not found in {this.reference.GetType().Name}.");
    }

    return this.reference[attributeName];
  }

  // private string MakeAttributeName() => this.Name.Split('.')[1];
}