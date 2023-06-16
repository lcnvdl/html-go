namespace HtmlRun.Common.Plugins.SQL;

public interface ISQLPlugin
{
  ISessionWrapper GetNewSession();

  ITransactionFactory GetNewTransactionFactory();

  void RunAndCommitTransaction(Action<ITransaction, ISessionWrapper> transactionToRun);
}

