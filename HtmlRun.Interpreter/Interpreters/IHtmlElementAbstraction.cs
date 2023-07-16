namespace HtmlRun.Interpreter.Interpreters;

public interface IHtmlElementAbstraction
{
  string? ElementId { get; set; }

  string TagName { get; }

  IEnumerable<IHtmlElementAbstraction> Children { get; }
  
  string InnerHtml { get; }

  string InnerText { get; }

  bool HasClass(string className, StringComparison comparison);

  string? GetData(string dataName);

  string? GetAttribute(string attributeName);

  IHtmlElementAbstraction? FindChildByTag(string tag, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) => this.Children.FirstOrDefault(m => m.TagName.Equals(tag, comparison));
}