using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.Native;

public interface INativeInstruction
{
  public string Key { get; }

  public Action<IRuntimeContext> Action { get; }
}