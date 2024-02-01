namespace HtmlRun.Common.Models;

public class CallArgumentModel : ICloneable
{
  public string? ArgumentType { get; set; }

  public string? Content { get; set; }

  public string? BranchCondition { get; set; }

  public string? Alias { get; set; }

  public string? Html { get; set; }

  public List<CallModel>? BranchInstructions { get; set; }

  public List<CallArgumentModel>? NestedArguments { get; set; }

  public bool IsString => this.ArgumentType == "string";

  public bool IsNumber => this.ArgumentType == "number";

  public bool IsPrimitive => this.IsString || this.IsNumber;

  public bool IsSolve => this.ArgumentType == "solve";

  public bool IsCall => this.ArgumentType == "call";

  public bool IsCallReference => this.ArgumentType == "callReference";

  public bool IsBranch => this.ArgumentType == "branch";

  public bool BranchIsEmpty => this.BranchInstructions == null || this.BranchInstructions.Count == 0;

  public static CallArgumentModel FromCall(string str)
  {
    return new CallArgumentModel() { ArgumentType = "call", Content = str };
  }

  public static CallArgumentModel FromString(string str)
  {
    return new CallArgumentModel() { ArgumentType = "string", Content = str };
  }

  public object Clone()
  {
    return new CallArgumentModel()
    {
      ArgumentType = this.ArgumentType,
      BranchCondition = this.BranchCondition,
      Content = this.Content,
      BranchInstructions = this.BranchInstructions?.Select(m => m).ToList(),
    };
  }

  public override string ToString()
  {
    return $"{this.Alias}: {this.ArgumentType} = {this.Content}";
  }
}

