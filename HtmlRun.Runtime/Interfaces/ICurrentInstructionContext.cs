namespace HtmlRun.Runtime.Interfaces;

public interface ICurrentInstructionContext : IBaseContext
{
  IRuntimeContext ParentContext { get; }

  string? GetArgument(int idx = 0) => GetArgument<string>(idx);

  string GetRequiredArgument(int idx = 0, string? errorMessage = null) => GetRequiredArgument<string>(idx, errorMessage);

  int CountArguments();

  T? GetArgument<T>(int idx = 0);

  T GetRequiredArgument<T>(int idx = 0, string? errorMessage = null);

  string?[] GetArguments();

  void Jump<T>(T jump) where T : class, IContextJump;

  Interfaces.IContextJump? CursorModification { get; set; }

  List<string> DirtyVariables { get; }
}