using System.Text.RegularExpressions;

namespace HtmlRun.SQL.NHibernate.Utils;

public static class SqlUtils
{
  public static string SelectLimitRows(string engine, string selectQuery, int limit)
  {
    switch (engine)
    {
      case Constants.DatabaseEngines.SQLServer:
        {
          var distinctMatch = Regex.Match(selectQuery, "^(SELECT\\s+DISTINCT)", RegexOptions.IgnoreCase);

          string keyword;

          if (distinctMatch.Success)
          {
            keyword = distinctMatch.Value;
          }
          else
          {
            keyword = "SELECT";
          }

          int index = selectQuery.IndexOf(keyword);

          string left = selectQuery.Remove(index + keyword.Length);
          string right = selectQuery.Substring(index + keyword.Length);

          return $"{left} TOP {limit}{right}";
        }

      case Constants.DatabaseEngines.Oracle:
        return $"SELECT * FROM ({selectQuery}) WHERE ROWNUM<={limit}";

      case Constants.DatabaseEngines.SQLite:
      case Constants.DatabaseEngines.MySQL:
        return $"{selectQuery} LIMIT {limit}";

      default:
        throw new NotImplementedException();
    }
  }

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
      if (string.IsNullOrEmpty(value))
      {
        return 0L;
      }

      return long.Parse(value);
    }

    if (toLower.Contains("small") || toLower == "short" || toLower == "tinyint")
    {
      if (string.IsNullOrEmpty(value))
      {
        return (short)0;
      }

      return short.Parse(value);
    }

    if (toLower.Contains("int") || (toLower == "number" && !value.Contains('.')))
    {
      if (string.IsNullOrEmpty(value))
      {
        return 0;
      }

      return int.Parse(value);
    }

    if (toLower.Contains("float") || toLower.Contains("numeric") || toLower.Contains("double") || toLower.Contains("real") || toLower.Contains("decimal") || (toLower == "number" && value.Contains('.')))
    {
      if (string.IsNullOrEmpty(value))
      {
        return 0m;
      }

      return decimal.Parse(value);
    }

    return value;
  }
}