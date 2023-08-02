using HtmlRun.Common.Models;

namespace HtmlRun.Runtime.Models;

class ApplicationStartedModel
{
  public InstructionPointer NewCursor { get; set; }

  public InstructionsGroup? MainGroup { get; set; }

  public List<CallModel> AppInstructions { get; set; }

  public ApplicationStartedModel(InstructionPointer cursor, InstructionsGroup main, List<CallModel> appInstructions)
  {
    this.NewCursor = cursor;
    this.MainGroup = main;
    this.AppInstructions = appInstructions;
  }
}