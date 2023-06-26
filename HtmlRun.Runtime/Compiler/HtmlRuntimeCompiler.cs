using HtmlRun.Common.Models;
using HtmlRun.Runtime.Factories;

namespace HtmlRun.Runtime;

static class HtmlRuntimeCompiler
{
  /// <summary>
  /// Following the interpretation, a parsing of the instructions is performed to transform them into a format 
  /// resembling 'machine language'. This process takes place after the interpretation phase, as the interpreter 
  /// is unaware of the specific instructions provided by the Providers.
  /// </summary>
  internal static void CompileInstructions(AppModel app)
  {
    Parallel.ForEach(app.InstructionGroups, group =>
    {
      CompileBranches(group);
    });
  }

  private static void CompileBranches(InstructionsGroup group)
  {
    var instructionsWithBranch = group.Instructions.FindAll(instruction => instruction.Arguments.Any(m => m.IsBranch && !m.BranchIsEmpty));

    foreach (var instructionWithBranch in instructionsWithBranch)
    {
      bool isIf = instructionWithBranch.FunctionName == Constants.BasicInstructionsSet.If;
      bool isSwitch = instructionWithBranch.FunctionName == Constants.BasicInstructionsSet.Switch;

      if (isIf || isSwitch)
      {
        string statement = instructionWithBranch.FunctionName;

        int idx = group.Instructions.IndexOf(instructionWithBranch);

        var newInstructions = new List<CallModel>();

        string key = $"{statement}-{group.Label}-{instructionWithBranch.Index}";

        var branches = instructionWithBranch.Arguments.FindAll(m => m.IsBranch && !m.BranchIsEmpty);

        var originalConditionArgument = instructionWithBranch.Arguments[0];

        //  Condition tests
        foreach (var branch in branches)
        {
          string expected = branch.BranchCondition!;
          CallArgumentModel conditionArgument;

          if (isSwitch)
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
            if (isIf)
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

          newInstructions.AddRange(branch.BranchInstructions!);

          newInstructions.Add(CallModelFactory.Goto($"end-{key}"));
        }

        //  End IF

        newInstructions.Add(CallModelFactory.Label($"end-{key}"));

        //  Add instructions and destroy branch

        group.Instructions.RemoveAt(idx);

        group.Instructions.InsertRange(idx, newInstructions);
      }
      else
      {
        throw new NotImplementedException();
      }
    }
  }

  private static CallArgumentModel GenerateArgumentForTestGotoForSwitchStatement(CallArgumentModel originalConditionArgument, string expectedValue)
  {
    var conditionArgument = (CallArgumentModel)originalConditionArgument.Clone();

    string val = conditionArgument.Content ?? string.Empty;

    if (conditionArgument.IsString)
    {
      val = $"`{val}`";
    }
    else
    {
      val = $"({val})";
    }

    conditionArgument.Content = $"{val}==`{expectedValue}`";
    conditionArgument.ArgumentType = "call";

    return conditionArgument;
  }
}
