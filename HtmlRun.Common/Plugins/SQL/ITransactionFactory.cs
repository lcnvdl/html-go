namespace HtmlRun.Common.Plugins.SQL;

public interface ITransactionFactory : IDisposable
{
  ITransaction GetNewTransaction();

  void RunTransaction(Action<ITransaction> transactionToRun);
}

