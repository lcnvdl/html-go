using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.Native;

public interface INativeJSInstruction
{
  Delegate ToJSAction();
}