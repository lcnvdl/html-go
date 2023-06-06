using HtmlRun.Common.Models;

public interface IInterpreter
{
  Task<AppModel> ParseString(string content);
}
