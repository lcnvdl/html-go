using System.Reflection;
using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.RuntimeContext;

public class ReflectionInferredContextValue : ContextValue
{
  private readonly object reference;

  private FieldInfo? fieldCache;

  public override bool IsInferred => true;

  private FieldInfo Field => this.fieldCache ??= this.GetFieldOrFail();

  public override string? Value
  {
    get
    {
      base.Value = this.Field.GetValue(this.reference)?.ToString();

      return base.Value;
    }

    set
    {
      base.Value = value;

      object? convertedValue = (value == null) ? null : Convert.ChangeType(value, this.Field!.FieldType);

      this.Field.SetValue(this.reference, convertedValue);
    }
  }

  public ReflectionInferredContextValue(object reference, string name) : base(name)
  {
    this.reference = reference;
  }

  public ReflectionInferredContextValue(object reference, string name, string value, bool isConst) : base(name, value, isConst)
  {
    this.reference = reference;
  }

  private FieldInfo GetFieldOrFail()
  {
    var calls = this.Name.Split('.').Skip(1).ToList();

    var type = this.reference.GetType();

    var field = type.GetField(calls[0]);

    for (int i = 1; i < calls.Count; i++)
    {
      if (field == null && i == 1)
      {
        throw new Exception($"Field {calls[i]} not found in {type.Name}.");
      }

      field = field!.FieldType.GetField(calls[i]);

      if (field == null)
      {
        throw new Exception($"Field {calls[i]} not found in {type.Name}.");
      }
    }

    return field!;
  }
}