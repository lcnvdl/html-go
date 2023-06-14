using HtmlRun.Common.Models;

namespace HtmlRun.Interpreter.Interpreters;

public interface IInterpreter
{
  Task<AppModel> ParseString(string content);
}
