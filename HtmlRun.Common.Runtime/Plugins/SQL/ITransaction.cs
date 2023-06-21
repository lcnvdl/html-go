using System.Reflection;

namespace HtmlRun.Common.Plugins.SQL;

public interface ITransaction : IDisposable
{
  void Commit();

  void Rollback();
}

