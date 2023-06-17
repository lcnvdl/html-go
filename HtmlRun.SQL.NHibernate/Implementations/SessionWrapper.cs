using HtmlRun.Common.Plugins.SQL;
using NH = NHibernate;

namespace HtmlRun.SQL.NHibernate;

sealed class SessionWrapper : ISessionWrapper, IDisposable
{
  public NH.ISession NativeSession { get; private set; }

  public string DatabaseEngine { get; private set; }

  public SessionWrapper(NH.ISession nativeSession, string dbEngine)
  {
    this.NativeSession = nativeSession;
    this.DatabaseEngine = dbEngine;
  }

  public void Dispose()
  {
    // this.NativeSession.Flush();
    this.NativeSession.Dispose();

    this.NativeSession = null!;

    GC.SuppressFinalize(this);
  }
}