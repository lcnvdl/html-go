namespace HtmlRun.Common.Models;

public class FunctionModel
{
  public string? Id { get; set; }

  public string? Code { get; set; }

  public List<string> Arguments { get; private set; } = new List<string>();
}

