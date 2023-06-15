using System.Reflection;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Common.Plugins.SQL;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace HtmlRun.SQL.NHibernate;

public class Plugin : PluginBase, ISQLPlugin, IOnApplicationStartPluginEvent, IBeforeApplicationLoadPluginEvent, IOnLoadEntityPluginEvent, IDisposable
{
  private ISessionFactory? sessionFactory;

  private readonly List<EntityRepository> repositories = new();

  public Plugin(Assembly appAssembly) : base(appAssembly)
  {
  }

  public void BeforeApplicationLoad(ApplicationStartEventModel data)
  {
  }

  public void OnApplicationStart(ApplicationStartEventModel data)
  {
    //  After entities are loaded

    this.sessionFactory = CreateSessionFactory();

    //  Test
    using var session = this.sessionFactory.OpenSession();
    using var transaction = session.BeginTransaction();

    var query = session.CreateSQLQuery("select 1+1");
    int number = Convert.ToInt32(query.UniqueResult());

    if (number != 2)
    {
      throw new Exception();
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

  public void Dispose()
  {
    this.sessionFactory?.Dispose();
  }

  private ISessionFactory CreateSessionFactory()
  {
    var cfg = new Configuration().DataBaseIntegration(db =>
      {
        db.ConnectionString = "FullUri=file:memorydb.db?mode=memory&cache=shared";
        db.Dialect<SQLiteDialect>();
        db.Driver<SQLite20Driver>();
      })
      .AddAssembly(this.AppAssembly);

    return cfg.BuildSessionFactory();
    // return Fluently.Configure().BuildSessionFactory();
  }
}

