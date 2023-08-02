using HtmlRun.Common.Models;

namespace HtmlRun.Runtime.Code;

public class GroupArguments
{
  public ParsedGroupArgument[]? Arguments { get; set; } = null;

  public bool HasArguments => this.Arguments != null && this.Arguments.Length > 0;

  public string Label { get; set; }

  public GroupArguments(string label)
  {
    this.Label = label;
  }

  public GroupArguments(string label, ParsedGroupArgument[] arguments)
    : this(label)
  {
    this.Arguments = arguments;
  }

  public static GroupArguments Empty(string label) => new GroupArguments(label);

  public static GroupArguments GetPartialWithValuesFromGroup(InstructionsGroup group, ParsedArgument[] args)
  {
    if (group.Arguments.Count != args.Length)
    {
      throw new InvalidOperationException("Arguments count mismatch.");
    }

    var argumentsArray = new ParsedGroupArgument[group.Arguments.Count];

    for (int i = 0; i < group.Arguments.Count; i++)
    {
      var arg = group.Arguments[i];
      argumentsArray[i] = new ParsedGroupArgument(i, arg.Name, args[i].Value, args[i].Type);
    }

    var groupArgs = new GroupArguments(group.Label, argumentsArray);
    return groupArgs;
  }
}
