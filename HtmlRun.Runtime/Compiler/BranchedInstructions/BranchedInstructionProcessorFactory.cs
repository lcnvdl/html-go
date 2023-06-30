using HtmlRun.Common.Models;
using HtmlRun.Runtime.Factories;

namespace HtmlRun.Runtime.Compiler.BranchedInstructions;

static class BranchedInstructionProcessorFactory
{
  internal static BranchedInstructionProcessor GetInstance(InstructionsGroup group, CallModel instruction)
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
      throw new NotImplementedException($"Unknown branched instruction '{instruction.FunctionName}'.");
    }
  }

  private static bool IsSelectionStatement(CallModel instruction)
  {
    bool isIf = instruction.FunctionName == Constants.BasicInstructionsSet.If;
    bool isSwitch = instruction.FunctionName == Constants.BasicInstructionsSet.Switch;
    bool isSelectionStatement = isIf || isSwitch;
    return isSelectionStatement;
  }

  private static bool IsIterationStatement(CallModel instruction)
  {
    bool isWhile = instruction.FunctionName == Constants.BasicInstructionsSet.While;
    bool isDoWhile = instruction.FunctionName == Constants.BasicInstructionsSet.DoWhile;
    bool isIterationStatement = isWhile || isDoWhile;
    return isIterationStatement;
  }
}