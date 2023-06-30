namespace HtmlRun.Runtime.RuntimeContext;

class JumpToLine : Interfaces.IContextJump
{
  public enum JumpTypeEnum
  {
    LineNumber,
    LineId,
  }

  public JumpToLine(int line) : this(line.ToString(), JumpTypeEnum.LineNumber, 0)
  {
  }

  public JumpToLine(string line, JumpTypeEnum jumpType, int offset = 0)
  {
    this.Line = line;
    this.JumpType = jumpType;
    this.Offset = offset;
  }

  public JumpTypeEnum JumpType { get; set; }

  public string? Line { get; set; }

  public int Offset { get; set; } = 0;

  public bool IsEmpty => string.IsNullOrEmpty(this.Line);
}