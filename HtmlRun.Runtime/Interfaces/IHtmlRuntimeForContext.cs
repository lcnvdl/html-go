using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Interfaces;

public interface IHtmlRuntimeForContext
{
  string? RunCallReference(ICurrentInstructionContext unsafeCtx, string referenceId);
}
