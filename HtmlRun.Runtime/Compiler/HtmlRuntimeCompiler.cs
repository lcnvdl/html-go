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
    //  Compile dependencies instructions // En principio no, porque cuando se llama a una librería hija directamente se va a usar otro HtmlRuntime.
    // Parallel.ForEach(app.Imports.Select(m => m.Library), CompileInstructions);

    //  Compile instructions
    Parallel.ForEach(app.InstructionGroups, group =>
    {
      AddGroupStartLabels(app, group);

      EnsureReturns(group);

      CompileBranches(group);

      AddGroupEndLabels(app, group);
    });

    //  Prepare main group for the merge
    var mainGroup = app.InstructionGroups.FirstOrDefault(m => m.IsMain);

    if (app.Type == AppType.Library)
    {
      if (mainGroup == null)
      {
        mainGroup = InstructionsGroup.Main;
        app.InstructionGroups.Insert(0, mainGroup);
      }
    }
    else
    {
      if (mainGroup == null)
      {
        throw new InvalidOperationException("Main group not found.");
      }
    }

    //  App goto end label
    mainGroup.Instructions.Add(CallModelFactory.Goto(CompilerConstants.ApplicationEndLabel(app)));

    //  Merge and clear groups
    foreach (var groupToMerge in app.LabeledGroups)
    {
      mainGroup.Instructions.AddRange(groupToMerge.Instructions);

      groupToMerge.Instructions.Clear();
    }

    //  No los remuevo porque se usan para poder encontrar por label
    // app.InstructionGroups.RemoveAll(m => !m.IsMain);

    //  Add application end
    mainGroup.Instructions.Add(CallModelFactory.Label(CompilerConstants.ApplicationEndLabel(app)));
  }

  private static void AddGroupStartLabels(AppModel app, InstructionsGroup group)
  {
    string key = CompilerConstants.GroupStartLabel(app, group);
    group.Instructions.Insert(0, CallModelFactory.Label(key));
  }

  private static void AddGroupEndLabels(AppModel app, InstructionsGroup group)
  {
    string key = CompilerConstants.GroupEndLabel(app, group);
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
