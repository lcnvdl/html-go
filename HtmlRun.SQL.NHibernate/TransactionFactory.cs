using HtmlRun.Common.Plugins.SQL;
using NH = NHibernate;

namespace HtmlRun.SQL.NHibernate;

public class TransactionFactory : ITransactionFactory, IDisposable
{
  private readonly NH.ISession session;

  public TransactionFactory(NH.ISession session)
  {
    this.session = session;
  }

  public void Dispose()
  {
    this.session.Dispose();
  }

  public ITransaction GetNewTransaction()
  {
    return new Transaction(this.session.BeginTransaction());
  }

  public void RunTransaction(Action<ITransaction> transactionToRun)
  {
    using var tx = this.GetNewTransaction();
    try
    {
      transactionToRun(tx);
      tx.Commit();
    }
    catch
    {
      tx.Rollback();
      throw;
    }
  }
}
