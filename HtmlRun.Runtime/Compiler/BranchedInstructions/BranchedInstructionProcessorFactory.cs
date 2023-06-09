using HtmlRun.Common.Models;

namespace HtmlRun.Runtime.Compiler.BranchedInstructions;

static class BranchedInstructionProcessorFactory
{
  internal static BranchedInstructionProcessor GetInstance(InstructionsGroup group, CallModel instruction)
  {
    BranchedInstructionProcessor? value = GetInstanceOrDefault(group, instruction);

    if (value == null)
    {
      throw new NotImplementedException($"Unknown branched instruction '{instruction.FunctionName}'.");
    }

    return value;
  }

  internal static BranchedInstructionProcessor? GetInstanceOrDefault(InstructionsGroup group, CallModel instruction)
  {
    if (IsSelectionStatement(instruction))
    {
      return new SelectionStatementBranchProcessor(group, instruction);
    }
    else if (IsIterationStatement(instruction))
    {
      return new IterationStatementBranchProcessor(group, instruction);
    }
    else
    {
      return null;
    }
  }

  private static bool IsSelectionStatement(CallModel instruction)
  {
    bool isSelectionStatement =
      instruction.FunctionName == Constants.BasicInstructionsSet.If ||
      instruction.FunctionName == Constants.BasicInstructionsSet.Switch;
    return isSelectionStatement;
  }

  private static bool IsIterationStatement(CallModel instruction)
  {
    bool isIterationStatement =
      instruction.FunctionName == Constants.BasicInstructionsSet.While ||
      instruction.FunctionName == Constants.BasicInstructionsSet.DoWhile ||
      instruction.FunctionName == Constants.BasicInstructionsSet.For;
    return isIterationStatement;
  }
}