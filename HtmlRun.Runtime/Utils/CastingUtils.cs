namespace HtmlRun.Runtime.Utils;

static class CastingUtils
{
  internal static CastingResult? ToNumber(string? val)
  {
    if (val == null)
    {
      return null;
    }

    if (val.Contains('.'))
    {
      if (decimal.TryParse(val, out var dresult))
      {
        return new CastingResult(typeof(decimal), dresult);
      }

      return null;
    }

    if (int.TryParse(val, out var result))
    {
      return new CastingResult(typeof(int), result);
    }

    if (long.TryParse(val, out var lresult))
    {
      return new CastingResult(typeof(long), lresult);
    }

    return null;
  }
}

class CastingResult
{
  public Type Type { get; set; }

  public object Value { get; set; }

  public CastingResult(Type type, object value)
  {
    Type = type;
    Value = value;
  }
}