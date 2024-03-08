using HtmlRun.Common.Models;
using HtmlRun.Interfaces;
using HtmlRun.Runtime.Exceptions;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

class InstructionPointer
{
  public int Position { get; private set; } = 0;

  public string ApplicationId { get; private set; }

  public Stack<IJumpWithMemory> CallStack { get; private set; } = new Stack<IJumpWithMemory>();

  public InstructionPointer(string applicationId, Stack<IJumpWithMemory>? callStack = null)
  {
    this.ApplicationId = applicationId;
    this.CallStack = callStack ?? new();
  }

  public void UnsafeRecoverFromApplicationContextChange()
  {
    var memory = this.CallStack.Pop() as IExternalJumpWithMemory ?? throw new InvalidOperationException("Call stack is empty.");
    this.Position = memory.CallPosition + 1;
    this.ApplicationId = memory.CallApplicationId;
  }

  public void MoveToNextPosition()
  {
    ++this.Position;
  }

  public bool IsPointingToSameApplication(AppModel? application)
  {
    //  TODO  Refactor this
    return !((application == null && !string.IsNullOrEmpty(this.ApplicationId)) || (application != null && this.ApplicationId != application.Id));
  }

  public void ApplyJumpOrFail(IContextJump? cursorModification, List<CallModel> instructions)
  {
    if (cursorModification is JumpToLine jump)
    {
      this.ApplyJump(jump, instructions);
    }
    else if (cursorModification is IJumpReturn)
    {
      if (this.CallStack.Count == 0)
      {
        throw new InvalidOperationException("Call stack is empty.");
      }

      if (this.CallStack.Peek() is IExternalJumpWithMemory)
      {
        //  TODO: Refactor this. Now it's being managed from the Unsafe method in HtmlRuntime.
        throw new JumpReturnTemporalException();
      }
      else
      {
        IJumpWithMemory call = this.CallStack.Pop()!;

        this.ApplyJump(new JumpToLine(call.CallPosition + 1), instructions);
      }
    }
    else
    {
      throw new InvalidCastException();
    }
  }

  public InstructionPointer Clone()
  {
    var clone = (InstructionPointer)base.MemberwiseClone();
    clone.CallStack = new Stack<IJumpWithMemory>(this.CallStack.Where(m => m is ICloneable).Cast<ICloneable>().Select(m => m.Clone()).Cast<IJumpWithMemory>());
    return clone;
  }

  private void ApplyJump(JumpToLine jump, List<CallModel> instructions)
  {
    bool isExternalJump = false;

    if (jump is IJumpWithMemory jumpWithMemory)
    {
      jumpWithMemory.CallPosition = this.Position;

      if (jumpWithMemory is IExternalJumpWithMemory externalJump)
      {
        externalJump.CallApplicationId = this.ApplicationId;
        this.ApplicationId = externalJump.ApplicationId;
        isExternalJump = true;
      }

      this.CallStack.Push(jumpWithMemory);
    }

    if (jump.JumpType == JumpToLine.JumpTypeEnum.LineNumber)
    {
      this.Position = int.Parse(jump.Line!) - 1 + jump.Offset;
    }
    else if (!isExternalJump)
    {
      this.Position = instructions.FindIndex(m => m.CustomId != null && m.CustomId == jump.Line) + jump.Offset;
      if (this.Position < 0)
      {
        throw new IndexOutOfRangeException($"Label {jump.Line} not found.");
      }

      --this.Position;
    }
  }
}