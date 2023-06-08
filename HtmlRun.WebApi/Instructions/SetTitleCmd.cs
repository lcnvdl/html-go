using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.WebApi.Instructions;

internal class SetTitleCmd : VoidInstruction
{
  internal SetTitleCmd() : base(Runtime.Constants.BasicInstructionsSet.SetTitle)
  {
  }
}