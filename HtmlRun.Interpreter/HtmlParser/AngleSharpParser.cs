using AngleSharp;
using AngleSharp.Dom;
using HtmlRun.Interpreter.Interpreters;

namespace HtmlRun.Interpreter.HtmlParser;

public class AngleSharpParser : IParser, IDisposable
{
  private IBrowsingContext? context;

  private IDocument? document;

  public AngleSharpParser()
  {
  }

  public string HeadTitle
  {
    get
    {
      if (this.document == null || this.document.Head == null)
      {
        return string.Empty;
      }

      string title = this.document.Head.Title ??
        this.document.Head.GetElementsByTagName("title").FirstOrDefault()?.InnerHtml ??
        string.Empty;

      return title.Trim();
    }
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

  public string? GetMetaContentWithDefaultValue(string name)
  {
    if (this.document == null)
    {
      throw new NullReferenceException();
    }

    var metatag = this.document.Head?.GetElementsByTagName("meta").FirstOrDefault(m => m.GetAttribute("name") == name);
    return metatag?.GetAttribute("content");
  }

  public string GetMetaContentWithDefaultValue(string name, string defaultValue)
  {
    return this.GetMetaContentWithDefaultValue(name) ?? defaultValue;
  }
}

class AngleSharpHtmlElementAbstraction : IHtmlElementAbstraction
{
  private IElement element;

  internal AngleSharpHtmlElementAbstraction(IElement element)
  {
    this.element = element;
  }

  public string? ElementId
  {
    get => this.element.Id;
    set => this.element.Id = value;
  }

  public string TagName => this.element.TagName;

  public string InnerHtml => this.element.InnerHtml;

  public string InnerText => this.element.InnerHtml;

  public IEnumerable<IHtmlElementAbstraction> Children => this.element.Children.AsEnumerable().Select(m => new AngleSharpHtmlElementAbstraction(m)).Cast<IHtmlElementAbstraction>();

  public bool HasClass(string className, StringComparison comparison) => this.element.ClassList.Any(m => className.Equals(m, comparison));

  public string? GetData(string dataName) => this.element.GetAttribute($"data-{dataName}");

  public override string ToString()
  {
    return this.element.ToHtml();
  }
}