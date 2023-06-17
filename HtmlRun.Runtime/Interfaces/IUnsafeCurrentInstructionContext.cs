using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Interfaces;

public interface IUnsafeCurrentInstructionContext
{
  IHtmlRuntimeForContext Runtime { get; }

  IHtmlRuntimeForUnsafeContext UnsafeRuntime { get; }

  void AddUsing(string namesp);

  void AddVariable(ContextValue value);
}