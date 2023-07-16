namespace HtmlRun.Common.Models;

public sealed class InstructionsGroup
{
  public const string MainLabel = "main";

  public string Label { get; set; } = string.Empty;

  public List<CallModel> Instructions { get; set; } = new();

  public static InstructionsGroup Main => new() { Label = MainLabel };
}

