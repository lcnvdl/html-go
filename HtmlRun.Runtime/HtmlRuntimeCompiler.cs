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
    foreach (var group in app.InstructionGroups)
    {
      var instructionsWithBranch = group.Instructions.FindAll(instruction => instruction.Arguments.Any(m => m.IsBranch && !m.BranchIsEmpty));

      foreach (var instructionWithBranch in instructionsWithBranch)
      {
        if (instructionWithBranch.FunctionName == Constants.BasicInstructionsSet.If)
        {
          int idx = group.Instructions.IndexOf(instructionWithBranch);

          var newInstructions = new List<CallModel>();

          string key = $"{group.Label}-{instructionWithBranch.Index}";

          var branches = instructionWithBranch.Arguments.FindAll(m => m.IsBranch && !m.BranchIsEmpty);

          //  Condition tests
          foreach (var branch in branches)
          {
            if (string.IsNullOrEmpty(branch.BranchCondition))
            {
              throw new Exception("If branch condition is empty.");
            }

            newInstructions.Add(CallModelFactory.TestAndGoto(instructionWithBranch.Arguments[0], branch.BranchCondition!, $"case-{key}-{branch.BranchCondition}"));
          }

          newInstructions.Add(CallModelFactory.Goto($"endif-{key}"));

          //  Branches instructions
          foreach (var branch in branches)
          {
            newInstructions.Add(CallModelFactory.Label($"case-{key}-{branch.BranchCondition}"));

            newInstructions.AddRange(branch.BranchInstructions!);

            newInstructions.Add(CallModelFactory.Goto($"endif-{key}"));
          }

          //  End IF

          newInstructions.Add(CallModelFactory.Label($"endif-{key}"));

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
  }
}
