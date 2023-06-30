using HtmlRun.Common.Models;
using HtmlRun.Runtime.Factories;

namespace HtmlRun.Runtime.Compiler.BranchedInstructions;

internal sealed class SelectionStatementBranchProcessor : BranchedInstructionProcessor
{
  private bool IsIf => this.InstructionWithBranch.FunctionName == Constants.BasicInstructionsSet.If;

  private bool IsSwitch => this.InstructionWithBranch.FunctionName == Constants.BasicInstructionsSet.Switch;

  public SelectionStatementBranchProcessor(InstructionsGroup group, CallModel instructionWithBranch) : base(group, instructionWithBranch)
  {
  }

  public override void VerifyAndAddLowLevelInstructions()
  {
    string statement = this.InstructionWithBranch.FunctionName;

    int originalInstructionIndex = this.Group.Instructions.IndexOf(this.InstructionWithBranch);

    var newInstructions = new List<CallModel>();

    string key = $"{statement}-{this.Group.Label}-{this.InstructionWithBranch.Index}";

    var branches = this.InstructionWithBranch.Arguments.FindAll(m => m.IsBranch && !m.BranchIsEmpty);

    var originalConditionArgument = this.InstructionWithBranch.Arguments[0];

    //  Condition tests
    foreach (var branch in branches)
    {
      string expected = branch.BranchCondition!;
      CallArgumentModel conditionArgument;

      if (this.IsSwitch)
      {
        //  Convert from case value (example: 'dog') to a comparation call (example: `dog`==`dog`).
        conditionArgument = GenerateArgumentForTestGotoForSwitchStatement(originalConditionArgument, expected);

        //  Expected is always true because the condition now compares against case value.
        expected = "true";
      }
      else
      {
        conditionArgument = originalConditionArgument;
      }

      if (string.IsNullOrEmpty(branch.BranchCondition))
      {
        if (this.IsIf)
        {
          throw new Exception($"If branch condition is empty.");
        }
        else
        {
          newInstructions.Add(CallModelFactory.Goto($"default-{key}"));
        }
      }
      else
      {
        newInstructions.Add(CallModelFactory.TestAndGoto(conditionArgument, expected, $"case-{key}-{branch.BranchCondition}"));
      }
    }

    newInstructions.Add(CallModelFactory.Goto($"end-{key}"));

    //  Branches instructions
    foreach (var branch in branches)
    {
      bool isDefault = string.IsNullOrEmpty(branch.BranchCondition);

      if (!isDefault)
      {
        newInstructions.Add(CallModelFactory.Label($"case-{key}-{branch.BranchCondition}"));
      }
      else
      {
        newInstructions.Add(CallModelFactory.Label($"default-{key}"));
      }

      newInstructions.Add(CallModelFactory.PushContext());
      newInstructions.AddRange(branch.BranchInstructions!);
      newInstructions.Add(CallModelFactory.PullContext());

      newInstructions.Add(CallModelFactory.Goto($"end-{key}"));
    }

    //  End IF

    newInstructions.Add(CallModelFactory.Label($"end-{key}"));

    //  Add instructions and destroy branch

    this.Group.Instructions.RemoveAt(originalInstructionIndex);

    this.Group.Instructions.InsertRange(originalInstructionIndex, newInstructions);
  }
}