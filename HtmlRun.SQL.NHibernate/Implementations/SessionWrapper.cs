using HtmlRun.Common.Plugins.SQL;
using NH = NHibernate;

sealed class SessionWrapper : ISessionWrapper, IDisposable
{
  public NH.ISession NativeSession { get; private set; }

  public SessionWrapper(NH.ISession nativeSession)
  {
    this.NativeSession = nativeSession;
  }

  public void Dispose()
  {
    // this.NativeSession.Flush();
    this.NativeSession.Dispose();

    this.NativeSession = null!;

    GC.SuppressFinalize(this);
  }
}