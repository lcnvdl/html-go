using HtmlRun.Common.Models;
using HtmlRun.Runtime.Factories;

namespace HtmlRun.Runtime.Compiler.BranchedInstructions;

internal sealed class IterationStatementBranchProcessor : BranchedInstructionProcessor
{
  public IterationStatementBranchProcessor(InstructionsGroup group, CallModel instructionWithBranch) : base(group, instructionWithBranch)
  {
  }

  public override void VerifyAndAddLowLevelInstructions()
  {
    string statement = this.InstructionWithBranch.FunctionName;

    int originalInstructionIndex = this.Group.Instructions.IndexOf(this.InstructionWithBranch);

    var newInstructions = new List<CallModel>();

    string key = $"{statement}-{this.Group.Label}-{this.InstructionWithBranch.Index}";

    var branches = this.InstructionWithBranch.Arguments.FindAll(m => m.IsBranch);

    if (branches.Count > 1)
    {
      throw new InvalidOperationException($"{statement} cannot have multiple branches.");
    }

    if (branches.Count == 0)
    {
      throw new InvalidOperationException($"{statement} branch is empty.");
    }

    var branch = branches.First();

    var originalConditionArgument = this.InstructionWithBranch.Arguments[0];

    //  Condition tests
    CallArgumentModel conditionArgument;

    conditionArgument = originalConditionArgument;

    if (!string.IsNullOrEmpty(branch.BranchCondition))
    {
      throw new Exception($"{statement} branch condition must be empty.");
    }

    newInstructions.Add(CallModelFactory.Label($"testloop-{key}"));

    newInstructions.Add(CallModelFactory.TestAndGoto(conditionArgument, "true", $"loop-{key}"));

    newInstructions.Add(CallModelFactory.Goto($"end-{key}"));

    //  Branches instructions

    newInstructions.Add(CallModelFactory.Label($"loop-{key}"));

    newInstructions.AddRange(branch.BranchInstructions!);

    newInstructions.Add(CallModelFactory.Goto($"testloop-{key}"));

    //  End while

    newInstructions.Add(CallModelFactory.Label($"end-{key}"));

    //  Add instructions and destroy branch

    this.Group.Instructions.RemoveAt(originalInstructionIndex);

    this.Group.Instructions.InsertRange(originalInstructionIndex, newInstructions);
  }
}