namespace HtmlRun.Runtime.RuntimeContext;

class JumpToLine : Interfaces.IContextJump
{
  public enum JumpTypeEnum
  {
    LineNumber,
    LineId,
  }

  public JumpToLine(string line, JumpTypeEnum jumpType)
  {
    this.Line = line;
    this.JumpType = jumpType;
  }

  public JumpTypeEnum JumpType { get; set; }

  public string? Line { get; set; }

  public bool IsEmpty => string.IsNullOrEmpty(this.Line);
}