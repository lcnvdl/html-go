namespace HtmlRun.Runtime.Code;

public class ParsedGroupArgument : ParsedArgument
{
  public int GroupIndex { get; set; }

  public string GroupArgumentName { get; set; }

  public ParsedGroupArgument(int index, string argName, string? value, ParsedArgumentType type)
    : base(value, type)
  {
    this.GroupIndex = index;
    this.GroupArgumentName = argName;
  }
}
