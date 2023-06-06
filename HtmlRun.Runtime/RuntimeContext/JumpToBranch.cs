namespace HtmlRun.Runtime.RuntimeContext;

class JumpToBranch : Interfaces.IContextJump
{
  public string? Condition { get; set; }

  public bool IsEmpty => string.IsNullOrEmpty(this.Condition);

  public JumpToBranch(string condition)
  {
    this.Condition = condition;
  }

  public bool IsBranch(string? branchCondition)
  {
    if (this.IsEmpty)
    {
      return false;
    }

    return this.Condition!.Equals(branchCondition, StringComparison.InvariantCultureIgnoreCase);
  }
}