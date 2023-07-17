using HtmlRun.Interfaces;

namespace HtmlRun.Runtime.RuntimeContext;

class JumpToLineWithCallStack : JumpToLine, IJumpWithMemory
{
  public JumpToLineWithCallStack(int line) : base(line)
  {
  }

  public JumpToLineWithCallStack(string line, JumpTypeEnum jumpType, int offset = 0)
    : base(line, jumpType, offset)
  {
  }

  public int CallPosition { get; set; }
}