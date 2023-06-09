namespace HtmlRun.Runtime.Code;

public class ParsedArgument
{
  public string? Value { get; private set; }

  public ParsedArgumentType Type { get; private set; }

  public bool IsNative => (this.Type & ParsedArgumentType.Native) == ParsedArgumentType.Native;

  public bool IsNull => this.Type == ParsedArgumentType.Null;

  public bool IsReference => this.Type == ParsedArgumentType.Reference;

  public ParsedArgument(string? value, ParsedArgumentType type)
  {
    this.Value = value;
    this.Type = type;
  }

  public static ParsedArgument Null => new ParsedArgument(null, ParsedArgumentType.Null);
}