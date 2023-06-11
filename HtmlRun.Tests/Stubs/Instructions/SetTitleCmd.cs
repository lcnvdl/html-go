using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Tests.Stubs.Instructions;

class SetTitleCmd : INativeInstruction
{
  public static string? LastTitle { get; set; }

  public string Key => Runtime.Constants.BasicInstructionsSet.SetTitle;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        LastTitle = ctx.GetRequiredArgument();
      };
    }
  }
}