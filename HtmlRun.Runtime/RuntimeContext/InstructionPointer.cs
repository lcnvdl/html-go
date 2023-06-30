using HtmlRun.Common.Models;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

class InstructionPointer
{
  public int Position { get; private set; } = 0;

  public void MoveToNextPosition()
  {
    ++this.Position;
  }

  public void ApplyJumpOrFail(IContextJump? cursorModification, List<CallModel> instructions)
  {
    if (cursorModification is JumpToLine jump)
    {
      this.ApplyJump(jump, instructions);
    }
    else
    {
      throw new InvalidCastException();
    }
  }

  private void ApplyJump(JumpToLine jump, List<CallModel> instructions)
  {
    if (jump.JumpType == JumpToLine.JumpTypeEnum.LineNumber)
    {
      this.Position = int.Parse(jump.Line!) - 1;
    }
    else
    {
      this.Position = instructions.FindIndex(m => m.CustomId != null && m.CustomId == jump.Line);
      if (this.Position < 0)
      {
        throw new IndexOutOfRangeException($"Label {jump.Line} not found.");
      }

      --this.Position;
    }
  }
}