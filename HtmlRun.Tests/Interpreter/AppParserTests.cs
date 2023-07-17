using HtmlRun.Common.Models;
using HtmlRun.Interpreter.HtmlParser;
using HtmlRun.Interpreter.Interpreters;
using HtmlRun.Interpreter.Interpreters.Parsers;

public class AppParserTests
{
  private IParser htmlParser;

  public AppParserTests()
  {
    this.htmlParser = new AngleSharpParser();
  }

  [Fact]
  public async void EntityParser_GetMetaContentWithDefaultValue_ShouldReturnNull_IfItDoesNotExists()
  {
    string page = PrepareTestPage("Test");

    await this.htmlParser.Load(page);

    string? nullValue = this.htmlParser.GetMetaContentWithDefaultValue("htmlgo:application-type");

    Assert.Null(nullValue!);
  }

  [Fact]
  public async void EntityParser_GetMetaContentWithDefaultValue_ShouldReturnDefault()
  {
    string page = PrepareTestPage("Test");

    await this.htmlParser.Load(page);

    string defaultValue = this.htmlParser.GetMetaContentWithDefaultValue("htmlgo:application-type", "Default");

    Assert.NotNull(defaultValue);
    Assert.Equal("Default", defaultValue);
  }

  [Fact]
  public async void EntityParser_GetMetaContentWithDefaultValue_ShouldWorkFine()
  {
    string page = PrepareTestPage("Version");

    await this.htmlParser.Load(page);

    var appVersionTag = this.htmlParser.GetMetaContentWithDefaultValue("htmlgo:application-version");

    Assert.NotNull(appVersionTag);
    Assert.Equal("1.0.1", appVersionTag);
  }

  [Fact]
  public async void EntityParser_Title_ShouldBeTrimmed()
  {
    string page = PrepareTestPage("  Employees ");

    await this.htmlParser.Load(page);

    Assert.NotNull(this.htmlParser.HeadTitle);
    Assert.Equal("Employees", this.htmlParser.HeadTitle);
  }

  private static string PrepareTestPage(string title)
  {
    string page = HtmlUtils.DefaultHtmlPage
        .Replace("$head", $"<title>{title}</title><meta name='htmlgo:application-version' content='1.0.1' />")
        .Replace("$body", "");
    return page;
  }
}
