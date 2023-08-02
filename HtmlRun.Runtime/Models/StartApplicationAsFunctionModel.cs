using HtmlRun.Common.Models;
using HtmlRun.Interfaces;
using HtmlRun.Runtime.Code;

namespace HtmlRun.Runtime.Models;

internal class StartApplicationAsFunctionModel
{
  public GroupArguments? ArgsAndValues { get; set; }

  public Stack<IJumpWithMemory>? CallStack { get; set; }
}
