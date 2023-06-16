using System.ComponentModel.DataAnnotations;

namespace HtmlRun.SQL.NHibernate.Utils;

public static class SqlUtils
{
  public static object SqlCast(string value, string sqlType)
  {
    string toLower = sqlType.ToLower();

    if (toLower == "bit" || toLower == "bool" || toLower == "boolean")
    {
      return value != "0" && value.ToLower() != "false";
    }

    if (toLower.Contains("char") || toLower.Contains("text"))
    {
      return value;
    }

    if (toLower.Contains("big") || toLower.Contains("long"))
    {
      return long.Parse(value);
    }

    if (toLower.Contains("small") || toLower == "short" || toLower == "tinyint")
    {
      return short.Parse(value);
    }

    if (toLower.Contains("int") || (toLower == "number" && !value.Contains('.')))
    {
      return int.Parse(value);
    }

    if (toLower.Contains("float") || toLower.Contains("numeric") || toLower.Contains("double") || toLower.Contains("real") || toLower.Contains("decimal") || (toLower == "number" && value.Contains('.')))
    {
      return decimal.Parse(value);
    }

    return value;
  }
}