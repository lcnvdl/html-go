namespace HtmlRun.Runtime.Native;

public interface INativeProvider
{
  public bool IsGlobal => string.IsNullOrEmpty(this.Namespace) || this.Namespace == Constants.Namespaces.Global;

  public string Namespace { get; }

  public INativeInstruction[] Instructions { get; }
}