using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Interfaces;

public interface IHtmlRuntimeForContext
{
  string[] Namespaces { get; }

  string? RunCallReference(ICurrentInstructionContext unsafeCtx, string referenceId);
}
