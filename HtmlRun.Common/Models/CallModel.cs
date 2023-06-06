namespace HtmlRun.Common.Models;

public class CallModel
{
  public string FunctionName { get; set; } = string.Empty;

  public List<CallArgumentModel> Arguments { get; set; } = new List<CallArgumentModel>();
}

