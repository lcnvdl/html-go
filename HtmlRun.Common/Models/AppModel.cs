namespace HtmlRun.Common.Models;

public class AppModel
{
  public string Title { get; set; } = string.Empty;

  public List<CallModel> Instructions { get; set; } = new List<CallModel>();

  public List<FunctionModel> Functions { get; set; } = new List<FunctionModel>();
}

