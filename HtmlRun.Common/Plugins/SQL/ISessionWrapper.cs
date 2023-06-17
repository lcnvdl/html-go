namespace HtmlRun.Common.Plugins.SQL;

public interface ISessionWrapper : IDisposable
{
  string DatabaseEngine { get; }
}