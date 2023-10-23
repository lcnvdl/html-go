namespace HtmlRun.Runtime.Native;

public interface IInstructionRequiresJsEngine
{
  void SetEngineGenerator(Func<object> engine);
}