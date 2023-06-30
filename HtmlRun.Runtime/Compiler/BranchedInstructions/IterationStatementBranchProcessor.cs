using HtmlRun.Common.Models;
using HtmlRun.Runtime.Factories;

namespace HtmlRun.Runtime.Compiler.BranchedInstructions;

internal sealed class IterationStatementBranchProcessor : BranchedInstructionProcessor
{
  private string Statement => this.InstructionWithBranch.FunctionName;

  private bool IsDoWhile => this.Statement == Constants.BasicInstructionsSet.DoWhile;

  private bool IsFor => this.Statement == Constants.BasicInstructionsSet.For;

  private string StatementInstructionKeyForId => $"{this.Statement}-{this.Group.Label}-{this.InstructionWithBranch.Index}";

  public IterationStatementBranchProcessor(InstructionsGroup group, CallModel instructionWithBranch) : base(group, instructionWithBranch)
  {
  }

  public override void VerifyAndAddLowLevelInstructions()
  {
    int originalInstructionIndex = this.Group.Instructions.IndexOf(this.InstructionWithBranch);

    var branches = this.GetAndValidateBranches();

    //  Condition tests

    var newInstructions = this.IsFor ? this.GenerateForInstructions(branches) : this.GenerateWhileInstructions(branches);

    //  Add instructions and destroy branch

    this.Group.Instructions.RemoveAt(originalInstructionIndex);

    this.Group.Instructions.InsertRange(originalInstructionIndex, newInstructions);
  }

  private List<CallModel> GenerateForInstructions(List<CallArgumentModel> branches)
  {
    var declarationBranch = branches[0];
    var incrementalBranch = branches[1];
    var iterationBranch = branches[2];
    var forCondition = this.InstructionWithBranch.Arguments.First(m => !m.IsBranch);

    string key = this.StatementInstructionKeyForId;
    var newInstructions = new List<CallModel>();

    newInstructions.Add(CallModelFactory.PushContext());
    newInstructions.AddRange(declarationBranch.BranchInstructions!);

    newInstructions.Add(CallModelFactory.Label($"testloop-{key}"));
    newInstructions.Add(CallModelFactory.TestAndGoto(forCondition, "true", $"loop-{key}"));
    newInstructions.Add(CallModelFactory.Goto($"end-{key}"));

    //  Branch instructions

    newInstructions.Add(CallModelFactory.Label($"loop-{key}"));
    newInstructions.Add(CallModelFactory.PushContext());
    newInstructions.AddRange(iterationBranch.BranchInstructions!);
    newInstructions.Add(CallModelFactory.PullContext());
    newInstructions.Add(CallModelFactory.PushContext());
    newInstructions.AddRange(incrementalBranch.BranchInstructions!);
    newInstructions.Add(CallModelFactory.PullContext());
    newInstructions.Add(CallModelFactory.Goto($"testloop-{key}"));

    //  End for

    newInstructions.Add(CallModelFactory.Label($"end-{key}"));
    newInstructions.Add(CallModelFactory.PullContext());

    return newInstructions;
  }

  private List<CallModel> GenerateWhileInstructions(List<CallArgumentModel> branches)
  {
    var originalConditionArgument = this.InstructionWithBranch.Arguments.First(m => !m.IsBranch);

    var branch = branches.First();
    CallArgumentModel conditionArgument;

    conditionArgument = originalConditionArgument;

    if (!string.IsNullOrEmpty(branch.BranchCondition))
    {
      throw new Exception($"{this.Statement} branch condition must be empty.");
    }

    string key = this.StatementInstructionKeyForId;
    var newInstructions = new List<CallModel>();

    if (this.IsDoWhile)
    {
      newInstructions.Add(CallModelFactory.Goto($"loop-{key}"));
    }

    newInstructions.Add(CallModelFactory.Label($"testloop-{key}"));
    newInstructions.Add(CallModelFactory.TestAndGoto(conditionArgument, "true", $"loop-{key}"));
    newInstructions.Add(CallModelFactory.Goto($"end-{key}"));

    //  Branch instructions

    newInstructions.Add(CallModelFactory.Label($"loop-{key}"));
    newInstructions.Add(CallModelFactory.PushContext());
    newInstructions.AddRange(branch.BranchInstructions!);
    newInstructions.Add(CallModelFactory.PullContext());
    newInstructions.Add(CallModelFactory.Goto($"testloop-{key}"));

    //  End while

    newInstructions.Add(CallModelFactory.Label($"end-{key}"));

    return newInstructions;
  }

  private List<CallArgumentModel> GetAndValidateBranches()
  {
    var branches = this.InstructionWithBranch.Arguments.FindAll(m => m.IsBranch);

    if (!this.IsFor)
    {
      if (branches.Count > 1)
      {
        throw new InvalidOperationException($"{this.Statement} cannot have multiple branches.");
      }

      if (branches.Count == 0)
      {
        throw new InvalidOperationException($"{this.Statement} branch is empty.");
      }
    }

    return branches;
  }
}
