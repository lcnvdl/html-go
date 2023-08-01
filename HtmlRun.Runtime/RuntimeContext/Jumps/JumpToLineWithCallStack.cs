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

class JumpToExternalLineWithCallStack : JumpToLine, IExternalJumpWithMemory
{
  public JumpToExternalLineWithCallStack(string appId, int line) : base(line)
  {
    this.ApplicationId = appId;
  }

  public JumpToExternalLineWithCallStack(string appId, string line, JumpTypeEnum jumpType, int offset = 0)
    : base(line, jumpType, offset)
  {
    this.ApplicationId = appId;
  }

  public int CallPosition { get; set; }

  public string CallApplicationId { get; set; } = "";

  public string ApplicationId { get; set; }
}