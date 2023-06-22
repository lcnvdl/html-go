using System.Text.RegularExpressions;
using HtmlRun.Common.Models;

namespace HtmlRun.Interpreter.Interpreters.Parsers;

public static class EntityParser
{
  public static EntityModel ParseTable(IHtmlElementAbstraction e)
  {
    IHtmlElementAbstraction thead = e.FindChildByTag("thead") ?? throw new Exception("Table tag thead is missing for entity.");
    IHtmlElementAbstraction tbody = e.FindChildByTag("tbody") ?? throw new Exception("Table tag tbody is missing for entity.");

    var model = new EntityModel();

    model.Name = thead.FindChildByTag("tr")?.FindChildByTag("th")?.InnerText?.Trim() ?? throw new Exception("Table name is required.");

    foreach (IHtmlElementAbstraction tr in tbody.Children.Where(m => m.TagName.Equals("tr", StringComparison.InvariantCultureIgnoreCase)))
    {
      if (!tr.HasClass("ignore", StringComparison.InvariantCultureIgnoreCase))
      {
        model.Attributes.Add(ParseTableAttribute(tr));
      }
    }

    return model;
  }

  private static EntityAttributeModel ParseTableAttribute(IHtmlElementAbstraction tr)
  {
    var children = tr.Children.ToList();

    var model = new EntityAttributeModel();

    model.Name = children[0].InnerText.Trim();
    model.SqlType = Regex.Match(children[1].InnerText, "^\\w+").Value;

    var lengthMatch = Regex.Match(children[1].InnerText, "\\d+");
    if (lengthMatch.Success)
    {
      model.Length = int.Parse(lengthMatch.Value);
    }

    model.IsNull = !Regex.IsMatch(children[2].InnerText, "NOT\\s+NULL", RegexOptions.IgnoreCase);
    model.IsPK = children[3].InnerText.Contains("PK");

    if (children.Count > 3)
    {
      string defaultVal = children[4].InnerText.Trim();

      if (!string.IsNullOrEmpty(defaultVal))
      {
        string defaultKeyword = "DEFAULT";

        if (defaultVal.StartsWith(defaultKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
          model.DefaultValue = defaultVal.Substring(defaultKeyword.Length).Trim();
        }
        else
        {
          model.DefaultValue = defaultVal;
        }
      }
    }

    return model;
  }
}
