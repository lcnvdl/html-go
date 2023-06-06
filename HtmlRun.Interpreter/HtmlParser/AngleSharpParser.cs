using AngleSharp;
using AngleSharp.Dom;

namespace HtmlRun.Interpreter.HtmlParser;

public class AngleSharpParser : IParser, IDisposable
{
  private IBrowsingContext? context;

  private IDocument? document;

  public AngleSharpParser()
  {
  }

  public string HeadTitle => this.document?.Head?.Title ?? string.Empty;

  public IEnumerable<IHtmlElementAbstraction> BodyQuerySelectorAll(string query)
  {
    if (this.document == null)
    {
      throw new NullReferenceException();
    }

    if (this.document.Body == null)
    {
      throw new NullReferenceException("HTML body tag is missing.");
    }

    var result = this.document.Body.QuerySelectorAll(query);
    return result.AsEnumerable().Select(m => new AngleSharpHtmlElementAbstraction(m)).Cast<IHtmlElementAbstraction>();
  }

  public async Task Load(string content)
  {
    var config = Configuration.Default.WithDefaultLoader();
    this.context = BrowsingContext.New(config);
    this.document = await context.OpenAsync(req => req.Content(content));
  }

  public void Dispose()
  {
    this.document?.Dispose();
    this.document = null;
    this.context?.Dispose();
    this.context = null;
  }
}

class AngleSharpHtmlElementAbstraction : IHtmlElementAbstraction
{
  private IElement element;

  internal AngleSharpHtmlElementAbstraction(IElement element)
  {
    this.element = element;
  }

  public string? ElementId => this.element.Id;

  public string TagName => this.element.TagName;

  public string InnerHtml => this.element.InnerHtml;

  public string InnerText => this.element.InnerHtml;

  public IEnumerable<IHtmlElementAbstraction> Children => this.element.Children.AsEnumerable().Select(m => new AngleSharpHtmlElementAbstraction(m)).Cast<IHtmlElementAbstraction>();

  public bool HasClass(string className, StringComparison comparison) => this.element.ClassList.Any(m => className.Equals(m, comparison));

  public override string ToString()
  {
    return this.element.ToHtml();
  }
}