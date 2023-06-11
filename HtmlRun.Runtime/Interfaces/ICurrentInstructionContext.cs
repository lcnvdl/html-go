using HtmlRun.Runtime.Code;

namespace HtmlRun.Runtime.Interfaces;

public interface ICurrentInstructionContext : IBaseContext
{
  IRuntimeContext ParentContext { get; }

  string? GetArgument(int idx = 0) => GetArgument<string>(idx);

  string GetRequiredArgument(int idx = 0, string? errorMessage = null) => GetRequiredArgument<string>(idx, errorMessage);

  ParsedArgument GetArgumentAt(int idx);

  int CountArguments();

  T? GetArgument<T>(int idx = 0);

  T GetRequiredArgument<T>(int idx = 0, string? errorMessage = null);

  ParsedArgument[] GetArguments();

  string?[] GetArgumentsValues() => GetArguments().Select(m => m.Value).ToArray();

  void Jump<T>(T jump) where T : class, IContextJump;

  Interfaces.IContextJump? CursorModification { get; set; }

  List<string> DirtyVariables { get; }
}