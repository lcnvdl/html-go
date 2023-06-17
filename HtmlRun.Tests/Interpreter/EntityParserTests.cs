using HtmlRun.Common.Models;
using HtmlRun.Interpreter.HtmlParser;
using HtmlRun.Interpreter.Interpreters;
using HtmlRun.Interpreter.Interpreters.Parsers;

public class EntityParserTests
{
  private IParser htmlParser;

  public EntityParserTests()
  {
    this.htmlParser = new AngleSharpParser();
  }

  [Fact]
  public async void EntityParser_ShouldWorkFine()
  {
    string page = HtmlUtils.DefaultHtmlPage
        .Replace("$head", "")
        .Replace("$body", "<table class='entity'><thead><tr><th colspan='5'>$tname</th></tr></thead><tbody>$tbody</tbody></table>")
        .Replace("$tname", "Users")
        .Replace("$tbody", "<tr> <td>Id</td>    <td>INT</td>          <td>NOT NULL</td> <td>PK</td>     <td></td>  </tr>$tbody")
        .Replace("$tbody", "<tr> <td>Email</td> <td>VARCHAR(100)</td> <td>NOT NULL</td> <td>Unique</td> <td></td>  </tr>$tbody")
        .Replace("$tbody", "<tr> <td>Name</td>  <td>VARCHAR(40)</td>  <td>NULL</td>     <td></td>       <td>DEFAULT NULL</td>  </tr>$tbody")
        .Replace("$tbody", "<!- tbody end -->");

    await this.htmlParser.Load(page);

    var entities = this.htmlParser.BodyQuerySelectorAll("table.entity")
      .Select(EntityParser.ParseTable)
      .Where(m => m != null)
      .Cast<EntityModel>()
      .ToList();

    Assert.NotNull(entities);
    Assert.Single(entities);
    Assert.Equal("Users", entities[0].Name);

    Assert.Equal("Id", entities[0].Attributes[0].Name);
    Assert.Equal("Email", entities[0].Attributes[1].Name);
    Assert.Equal("Name", entities[0].Attributes[2].Name);

    Assert.True(entities[0].Attributes[0].IsPK);
    Assert.False(entities[0].Attributes[1].IsPK);
    Assert.False(entities[0].Attributes[2].IsPK);

    Assert.Null(entities[0].Attributes[0].DefaultValue);
    Assert.Null(entities[0].Attributes[1].DefaultValue);
    Assert.Equal("NULL", entities[0].Attributes[2].DefaultValue);
  }
}