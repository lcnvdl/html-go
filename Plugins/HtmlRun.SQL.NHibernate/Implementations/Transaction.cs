using HtmlRun.Common.Plugins.SQL;
using NH = NHibernate;

namespace HtmlRun.SQL.NHibernate;

public class Transaction : ITransaction, IDisposable
{
  private readonly NH.ITransaction transaction;

  public Transaction(NH.ITransaction transaction)
  {
    this.transaction = transaction;
  }

  public void Commit()
  {
    this.transaction.Commit();
  }

  public void Rollback()
  {
    this.transaction.Rollback();
  }

  public void Dispose()
  {
    this.transaction.Dispose();
    GC.SuppressFinalize(this);
  }
}

