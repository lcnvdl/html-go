namespace HtmlRun.Common.Models;

public class CallArgumentModel
{
  public string? ArgumentType { get; set; }

  public string? Content { get; set; }

  public bool IsString => this.ArgumentType == "string";

  public bool IsNumber => this.ArgumentType == "number";

  public bool IsPrimitive => this.IsString || this.IsNumber;

  public bool IsSolve => this.ArgumentType == "solve";

  public bool IsCall => this.ArgumentType == "call";

  public bool IsBranch => this.ArgumentType == "branch";

  public string? BranchCondition { get; set; }

  public List<CallModel>? BranchInstructions { get; set; }

  public static CallArgumentModel FromString(string str)
  {
    return new CallArgumentModel() { ArgumentType = "string", Content = str };
  }
}

