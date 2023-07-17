namespace HtmlRun.Runtime.Native;

public interface INativeJSInstruction : INativeJSBaseInstruction
{
  Delegate ToJSAction();
}