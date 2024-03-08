namespace HtmlRun.Common.Models;

public class CallModel
{
  public int Index { get; set; }

  public string? CustomId { get; set; }

  public string? TaskId { get; set; }

  public string FunctionName { get; set; } = string.Empty;

  public List<CallArgumentModel> Arguments { get; set; } = new List<CallArgumentModel>();

  public bool IsAsync => !string.IsNullOrEmpty(this.TaskId);

  public bool IsSpecial => this.CallType == CallType.Special;

  public CallType CallType { get; set; }

  public override string ToString()
  {
    return $"#{this.Index}# {FunctionName} ({string.Join(", ", this.Arguments.Select(m => m.ToString()))})";
  }
}

