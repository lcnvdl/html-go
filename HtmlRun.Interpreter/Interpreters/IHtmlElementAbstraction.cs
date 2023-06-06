public interface IHtmlElementAbstraction
{
  string? ElementId { get; }

  string TagName { get; }

  IEnumerable<IHtmlElementAbstraction> Children { get; }
  
  string InnerHtml { get; }

  string InnerText { get; }

  bool HasClass(string className, StringComparison comparison);
}