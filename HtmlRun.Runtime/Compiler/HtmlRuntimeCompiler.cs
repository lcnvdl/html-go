using HtmlRun.Common.Models;
using HtmlRun.Runtime.Compiler.BranchedInstructions;

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
