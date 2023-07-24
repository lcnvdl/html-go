using HtmlRun.Common.Plugins.SQL;
using NH = NHibernate;

namespace HtmlRun.SQL.NHibernate.Factories;

public class TransactionFactory : ITransactionFactory, IDisposable
{
  private readonly NH.ISession session;

  private ISessionWrapper? sessionWrapper;

  private readonly string dbEngine;

  public TransactionFactory(NH.ISession session, string dbEngine)
  {
    this.session = session;
    this.dbEngine = dbEngine;
  }

  public ISessionWrapper Session => this.sessionWrapper ?? (this.sessionWrapper = new SessionWrapper(session, this.dbEngine));

  public void Dispose()
  {
    this.Session.Dispose();
    this.sessionWrapper = null;

    GC.SuppressFinalize(this);
  }

  public ITransaction GetNewTransaction()
  {
    return new Transaction(this.session.BeginTransaction());
  }

  public void RunAndCommitTransaction(Action<ITransaction, ISessionWrapper> transactionToRun)
  {
    using var tx = this.GetNewTransaction();
    try
    {
      transactionToRun(tx, this.Session);
      tx.Commit();
    }
    catch
    {
      tx.Rollback();
      throw;
    }
  }
}
