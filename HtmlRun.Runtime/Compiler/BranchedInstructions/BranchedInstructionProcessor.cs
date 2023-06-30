using HtmlRun.Common.Models;
using HtmlRun.Runtime.Factories;

namespace HtmlRun.Runtime.Compiler.BranchedInstructions;

internal abstract class BranchedInstructionProcessor
{
  protected InstructionsGroup Group { get; private set; }

  protected CallModel InstructionWithBranch { get; private set; }

  public BranchedInstructionProcessor(InstructionsGroup group, CallModel instructionWithBranch)
  {
    this.Group = group;
    this.InstructionWithBranch = instructionWithBranch;
  }

  public abstract void VerifyAndAddLowLevelInstructions();

  protected static CallArgumentModel GenerateArgumentForTestGotoForSwitchStatement(CallArgumentModel originalConditionArgument, string expectedValue)
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