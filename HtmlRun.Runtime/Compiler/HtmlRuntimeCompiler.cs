using HtmlRun.Common.Models;
using HtmlRun.Runtime.Compiler.BranchedInstructions;
using HtmlRun.Runtime.Constants;
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
      AddGroupStartLabels(group);

      EnsureReturns(group);

      CompileBranches(group);

      AddGroupEndLabels(group);
    });

    //  Prepare main group for the merge
    var mainGroup = app.InstructionGroups.First(m => m.IsMain);
    mainGroup.Instructions.Add(CallModelFactory.Goto("application-end"));

    //  Merge and clear groups
    foreach (var groupToMerge in app.LabeledGroups)
    {
      mainGroup.Instructions.AddRange(groupToMerge.Instructions);

      groupToMerge.Instructions.Clear();
    }

    //  No los remuevo porque se usan para poder encontrar por label
    // app.InstructionGroups.RemoveAll(m => !m.IsMain);

    //  Add application end
    mainGroup.Instructions.Add(CallModelFactory.Label("application-end"));
  }

  private static void AddGroupStartLabels(InstructionsGroup group)
  {
    string key = $"group-{group.Label}";
    group.Instructions.Insert(0, CallModelFactory.Label(key));
  }

  private static void AddGroupEndLabels(InstructionsGroup group)
  {
    string key = $"end-group-{group.Label}";
    group.Instructions.Add(CallModelFactory.Label(key));
  }

  private static void EnsureReturns(InstructionsGroup group)
  {
    if (!group.IsMain)
    {
      bool isReturn = group.Instructions.Last().FunctionName == BasicInstructionsSet.Return;

      if (!isReturn)
      {
        group.Instructions.Add(CallModelFactory.Return());
      }
    }
  }

  private static void CompileBranches(InstructionsGroup group)
  {
    int count;

    do
    {
      count = group.Instructions.Count;
      var instructionsWithBranch = group.Instructions.FindAll(instruction => instruction.Arguments.Any(m => m.IsBranch && !m.BranchIsEmpty));
      CompileBranches(group, instructionsWithBranch, false);
    }
    while (count != group.Instructions.Count);
  }

  private static void CompileBranches(InstructionsGroup group, List<CallModel> instructionsWithBranch, bool branchIsOptional)
  {
    foreach (CallModel instruction in instructionsWithBranch)
    {
      // var branches = instruction.Arguments.FindAll(m => m.IsBranch && !m.BranchIsEmpty);
      // foreach (var branch in branches)
      // {
      //   CompileBranches(group, branch.BranchInstructions!, true);
      // }

      var processor = branchIsOptional ?
        BranchedInstructionProcessorFactory.GetInstanceOrDefault(group, instruction) :
        BranchedInstructionProcessorFactory.GetInstance(group, instruction);
      processor!.VerifyAndAddLowLevelInstructions();
    }
  }
}
