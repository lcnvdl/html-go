namespace HtmlRun.Common.Plugins.SQL;

public interface ISQLPlugin
{
  IEntityRepository GetRepository(string name);

  ISessionWrapper GetNewSession();

  ITransactionFactory GetNewTransactionFactory();

  void RunAndCommitTransaction(Action<ITransaction, ISessionWrapper> transactionToRun);
}

