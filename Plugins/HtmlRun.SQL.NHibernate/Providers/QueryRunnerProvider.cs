using System.Text.Json;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.SQL.NHibernate.Extensions;

namespace HtmlRun.SQL.NHibernate.Providers;

public class QueryRunnerProvider : INativeProvider
{
  public string Namespace => "QueryRunner";

  public INativeInstruction[] Instructions => new INativeInstruction[]
  {
    new RunQueryAndGetUniqueValue(),
    new RunQueryAndGetListCmd(),
  };
}

class RunQueryAndGetUniqueValue : INativeInstruction, INativeJSInstruction
{
  public string Key => "RunQueryAndGetUniqueValue";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(sql => RunQuery(sql));
  }

  private static string RunQuery(string query)
  {
    using var session = Plugin.Instance.GetNewSession();
    object value = session.ExecuteSQLQueryAndGetUniqueResult(query);
    return value.ToString()!;
  }
}

class RunQueryAndGetListCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "RunQueryAndGetList";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(sql => RunQuery(sql));
  }

  private static string RunQuery(string query)
  {
    using var session = Plugin.Instance.GetNewSession();
    var values = session.ExecuteSQLQuery(query);
    return JsonSerializer.Serialize(values);
  }
}
