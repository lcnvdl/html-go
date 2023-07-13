namespace HtmlRun.Interpreter.Interpreters;

public interface IParser
{
  Task Load(string content);

  IEnumerable<IHtmlElementAbstraction> BodyQuerySelectorAll(string query);

  string? GetMetaContentWithDefaultValue(string name);

  string GetMetaContentWithDefaultValue(string name, string defaultValue);

  string HeadTitle { get; }
}