namespace HtmlRun.Runtime.Interfaces;

public interface IRuntimeContext
{
  string? GetArgument(int idx = 0, string? errorMessage = null) => GetArgument<string>(idx, errorMessage);

  T? GetArgument<T>(int idx = 0, string? errorMessage = null);

  string?[] GetArguments();

  void JumpToBranch(string condition);
}