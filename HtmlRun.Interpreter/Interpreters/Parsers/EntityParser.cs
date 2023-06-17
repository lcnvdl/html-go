using System.Text.RegularExpressions;
using HtmlRun.Common.Models;

namespace HtmlRun.Interpreter.Interpreters.Parsers;

public static class EntityParser
{
  public static EntityModel ParseTable(IHtmlElementAbstraction e)
  {
    IHtmlElementAbstraction thead = e.FindChildByTag("thead") ?? throw new Exception("Table tag thead is missing for entity.");
    IHtmlElementAbstraction tbody = e.FindChildByTag("tbody") ?? throw new Exception("Table tag tbody is missing for entity.");

    EntityModel model = new();

    model.Name = thead.FindChildByTag("tr")?.FindChildByTag("th")?.InnerText ?? throw new Exception("Table name is required.");

    foreach (var tr in tbody.Children.Where(m => m.TagName.Equals("tr", StringComparison.InvariantCultureIgnoreCase)))
    {
      model.Attributes.Add(ParseTableAttribute(tr));
    }

    return model;
  }

  private static EntityAttributeModel ParseTableAttribute(IHtmlElementAbstraction tr)
  {
    var children = tr.Children.ToList();

    EntityAttributeModel model = new();

    string defaultKeyword = "DEFAULT";

    model.Name = children[0].InnerText;
    model.SqlType = children[1].InnerText;
    model.IsNull = !Regex.IsMatch(children[2].InnerText, "NOT\\s+NULL", RegexOptions.IgnoreCase);
    model.IsPK = children[3].InnerText.Contains("PK");

    if (children.Count > 3)
    {
      string defaultVal = children[4].InnerText.Trim();

      if (defaultVal.StartsWith(defaultKeyword, StringComparison.InvariantCultureIgnoreCase))
      {
        model.DefaultValue = defaultVal.Substring(defaultKeyword.Length).Trim();
      }
    }

    return model;
  }
}
