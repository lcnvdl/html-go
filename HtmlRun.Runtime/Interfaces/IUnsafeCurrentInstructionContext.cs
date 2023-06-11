namespace HtmlRun.Runtime.Interfaces;

public interface IUnsafeCurrentInstructionContext
{
  IHtmlRuntimeForContext Runtime { get; }

  void AddUsing(string namesp);
}