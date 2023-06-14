namespace HtmlRun.Interpreter.Interpreters;

public interface IParser
{
  Task Load(string content);

  IEnumerable<IHtmlElementAbstraction> BodyQuerySelectorAll(string query);
  
  string HeadTitle { get; }
}