using System.Reflection;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.SQL.NHibernate.Factories;
using NHibernate;

namespace HtmlRun.SQL.NHibernate;

public class Plugin : PluginBase, ISQLPlugin, IOnApplicationStartPluginEvent, IBeforeApplicationLoadPluginEvent, IOnLoadEntityPluginEvent, IDisposable
{
  private ISessionFactory? sessionFactory;

  private readonly List<EntityRepository> repositories = new();

  private readonly PluginSettings settings = new();

  public Plugin(Assembly appAssembly) : base(appAssembly)
  {
  }

  public void BeforeApplicationLoad(ApplicationStartEventModel data)
  {
    //  Load settings

    this.settings.Load(data.EnvironmentVariables);
  }

  public void OnApplicationStart(ApplicationStartEventModel data)
  {
    //  After entities are loaded

    this.sessionFactory = FactoryForISessionFactory.CreateSessionFactory(this.settings);

    //  Test

    if (this.settings.TestDatabaseAfterConnection)
    {
      using var session = this.sessionFactory.OpenSession();
      using var transaction = session.BeginTransaction();

      var query = session.CreateSQLQuery("select 1+1");
      int number = Convert.ToInt32(query.UniqueResult());

      if (number != 2)
      {
        throw new Exception();
      }
    }
  }

  public void OnLoadEntity(EntityModel entity)
  {
    repositories.Add(new EntityRepository(entity));
  }

  public EntityRepository GetRepository(string name)
  {
    return this.repositories.First(m => m.Entity.Name == name);
  }

  public ISessionWrapper GetNewSession()
  {
    return new SessionWrapper(this.sessionFactory!.OpenSession());
  }

  public ITransactionFactory GetNewTransactionFactory()
  {
    return new TransactionFactory(this.sessionFactory!.OpenSession());
  }

  public void RunAndCommitTransaction(Action<Common.Plugins.SQL.ITransaction, ISessionWrapper> transactionToRun)
  {
    using var factory = this.GetNewTransactionFactory();
    factory.RunAndCommitTransaction(transactionToRun);
  }

  public void Dispose()
  {
    this.sessionFactory?.Close();
    this.sessionFactory?.Dispose();
    this.sessionFactory = null;

    GC.SuppressFinalize(this);
  }
}

