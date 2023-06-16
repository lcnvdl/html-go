using System.Data.Common;
using HtmlRun.Common.Plugins.SQL;
using NHibernate;

namespace HtmlRun.SQL.NHibernate.Extensions;

public static class SessionQueryRunner
{
  public static void ExecuteNonQuery(this ISessionWrapper sessionWrapper, string query)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;

    var command = session.Connection.CreateCommand();
    command.CommandText = query;

    Enlist(session, command);

    command.ExecuteNonQuery();
  }

  public static bool Enlist(this ISession session, DbCommand command, bool verifyTransactionStatus = false)
  {
    if (session.GetCurrentTransaction() != null)
    {
      if (verifyTransactionStatus && !session.GetCurrentTransaction().IsActive)
      {
        session.GetCurrentTransaction().Begin();
        return true;
      }

      session.GetCurrentTransaction().Enlist(command);
    }

    return false;
  }
}