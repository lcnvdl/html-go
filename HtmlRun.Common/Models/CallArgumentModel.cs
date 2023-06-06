namespace HtmlRun.Common.Models;

public class CallArgumentModel
{
  public string? ArgumentType { get; set; }

  public string? Content { get; set; }

  public bool IsString => this.ArgumentType == "string";

  public bool IsSolve => this.ArgumentType == "solve";

  public bool IsCall => this.ArgumentType == "call";

  public static CallArgumentModel FromString(string str)
  {
    return new CallArgumentModel() { ArgumentType = "string", Content = str };
  }
}

