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

  public void ApplyJumpOrFail(IContextJump? cursorModification, CallModel instruction, List<CallModel> instructions)
  {
    if (cursorModification is JumpToBranch)
    {
      var jump = (JumpToBranch)cursorModification;
      this.ApplyJump(jump, instruction, instructions);
    }
    else if (cursorModification is JumpToLine)
    {
      var jump = (JumpToLine)cursorModification;
      this.ApplyJump(jump, instructions);
    }
    else
    {
      throw new InvalidCastException();
    }
  }

  private void ApplyJump(JumpToBranch jump, CallModel currentInstruction, List<CallModel> instructions)
  {
    var newBranch = currentInstruction.Arguments.Find(m => m.IsBranch && jump.IsBranch(m.BranchCondition));
    if (newBranch == null)
    {
      // if (jump.IsBranchRequired) // Bug 01
      // {
      throw new NullReferenceException($"Missing branch {jump.Condition} in {currentInstruction.FunctionName} call.");
      // }
    }
    else
    {
      //  TODO Causa del Bug 01 (me obliga a tener el case false en el IF, sino quedan instrucciones sueltas)
      instructions.InsertRange(this.Position + 1, newBranch.BranchInstructions!);
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