namespace HtmlRun.Common.Models;

public sealed class InstructionsGroup
{
  public const string MainLabel = "main";

  public static InstructionsGroup Main => new(MainLabel);

  public string Label { get; set; } = string.Empty;

  public List<CallModel> Instructions { get; private set; } = new();

  public List<InstructionGroupArgumentModel> Arguments { get; private set; } = new();

  public bool IsMain => this.Label.Equals(MainLabel, StringComparison.InvariantCultureIgnoreCase);

  public bool HasArguments => this.Arguments.Count > 0;

  public InstructionsGroup(string label)
  {
    this.Label = label;
  }

  public override string ToString() => this.Label;
}

