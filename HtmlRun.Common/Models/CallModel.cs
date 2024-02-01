namespace HtmlRun.Common.Models;

public class CallModel
{
  public int Index { get; set; }

  public string? CustomId { get; set; }

  public string FunctionName { get; set; } = string.Empty;

  public List<CallArgumentModel> Arguments { get; set; } = new List<CallArgumentModel>();

  public bool IsSpecial { get; set; } = false;

  public override string ToString()
  {
    return $"#{this.Index}# {FunctionName} ({string.Join(", ", this.Arguments.Select(m => m.ToString()))})";
  }
}

