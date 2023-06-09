using System.Reflection;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.Runtime.Native;
using HtmlRun.SQL.NHibernate.Factories;
using HtmlRun.SQL.NHibernate.Providers;
using NHibernate;

namespace HtmlRun.SQL.NHibernate;

public class Plugin : PluginBase, ISQLPlugin, IOnApplicationStartPluginEvent, IBeforeApplicationLoadPluginEvent, IOnLoadEntityPluginEvent, IDisposable
{
  //  TODO: Improve the plugin system to get the plugin instance in the context (useful for providers).
  internal static Plugin Instance { get; private set; }

  private ISessionFactory? sessionFactory;

  private readonly Dictionary<string, IEntityRepository> repositories = new();

  private readonly PluginSettings settings = new();

  public override INativeProvider[]? Providers => new INativeProvider[] { new QueryRunnerProvider(), new RepositoryProvider(), new DDLProvider(), };

  internal PluginSettings Settings => this.settings;

  public Plugin(Assembly appAssembly) : base(appAssembly)
  {
    Instance = this;
  }

  public void BeforeApplicationLoad(ApplicationStartEventModel data)
  {
    //  Load settings

    this.settings.Load(data.EnvironmentVariables);

    //  Load DLLs

    //  TODO  Remove this verification

    if (System.Data.SQLite.SQLiteFactory.Instance == null)
    {
      throw new Exception("SQLite not installed.");
    }
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
    repositories[entity.Name] = new EntityRepository(entity);
  }

  public IEntityRepository GetRepository(string name)
  {
    return this.repositories[name];
  }

  public ISessionWrapper GetNewSession()
  {
    return new SessionWrapper(this.sessionFactory!.OpenSession(), this.settings.DatabaseLibrary);
  }

  public ITransactionFactory GetNewTransactionFactory()
  {
    return new TransactionFactory(this.sessionFactory!.OpenSession(), this.settings.DatabaseLibrary);
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

