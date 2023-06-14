using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Interfaces;

public interface IRuntimeContext : IBaseContext
{
  List<string> Usings { get; }

  void AddUsing(string namesp);
  
  void AddVariable(ContextValue value);

  ICurrentInstructionContext Fork(IHtmlRuntimeForContext runtimeForContext, string callName, IEnumerable<ParsedArgument> args);
}