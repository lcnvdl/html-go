using HtmlRun.Common.Models;
using HtmlRun.Interfaces;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

class InstructionPointer
{
  public int Position { get; private set; } = 0;

  public Stack<IJumpWithMemory> CallStack { get; private set; } = new Stack<IJumpWithMemory>();

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
    else if (cursorModification is IJumpReturn)
    {
      var call = this.CallStack.Pop() ?? throw new InvalidOperationException("Call stack is empty.");
      
      this.ApplyJump(new JumpToLine(call.CallPosition + 1), instructions);
    }
    else
    {
      throw new InvalidCastException();
    }
  }

  private void ApplyJump(JumpToLine jump, List<CallModel> instructions)
  {
    if (jump is IJumpWithMemory jumpWithMemory)
    {
      jumpWithMemory.CallPosition = this.Position;
      this.CallStack.Push(jumpWithMemory);
    }

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