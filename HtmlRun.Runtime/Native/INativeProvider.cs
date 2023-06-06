namespace HtmlRun.Runtime.Native;

public interface INativeProvider
{
  public string Namespace { get; }

  public INativeInstruction[] Instructions { get; }
}