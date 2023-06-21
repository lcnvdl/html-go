namespace HtmlRun.Common.Plugins.SQL;

public interface ITransactionFactory : IDisposable
{
  ISessionWrapper Session { get; }

  ITransaction GetNewTransaction();

  void RunAndCommitTransaction(Action<ITransaction, ISessionWrapper> transactionToRun);
}

