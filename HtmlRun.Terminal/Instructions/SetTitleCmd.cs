using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Terminal.Instructions;

class SetTitleCmd : INativeInstruction
{
  public string Key => Runtime.Constants.BasicInstructionsSet.SetTitle;

  public Action<IRuntimeContext> Action
  {
    get
    {
      return ctx =>
      {
        var arg = ctx.GetArgument();
        if (!string.IsNullOrEmpty(arg))
        {
          Console.Title = arg;
        }
      };
    }
  }
}