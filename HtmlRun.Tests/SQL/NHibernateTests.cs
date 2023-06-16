using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.SQL.NHibernate;
using HtmlRun.SQL.NHibernate.Extensions;

public class NHibernateTests : IDisposable
{
  private Plugin plugin;

  private EntityRepository repo;

  private ISessionWrapper session;

  private static EntityModel TestEntity => new EntityModel()
  {
    Name = "Test",
    Attributes = new List<EntityAttributeModel>()
      {
        new EntityAttributeModel() { Name = "Id", SqlType = "INTEGER"},
        new EntityAttributeModel() { Name = "Name", SqlType = "TEXT"},
      }
  };

  public NHibernateTests()
  {
    this.plugin = new(GetType().Assembly);

    this.plugin.BeforeApplicationLoad(new ApplicationStartEventModel(Environment.GetEnvironmentVariables().ToDictionary<string, string>()));

    this.plugin.OnLoadEntity(TestEntity);

    this.plugin.OnApplicationStart(new ApplicationStartEventModel(Environment.GetEnvironmentVariables().ToDictionary<string, string>()));

    this.repo = plugin.GetRepository("Test");

    this.session = this.plugin.GetNewSession();

    this.session.ExecuteNonQuery("CREATE TABLE Test ( Id INTEGER PRIMARY KEY, Name TEXT )");

    // this.plugin.RunAndCommitTransaction((tx, session) => session.ExecuteNonQuery("CREATE TABLE Test ( Id INTEGER PRIMARY KEY, Name TEXT )"));
  }

  public void Dispose()
  {
    try
    {
      using var session = plugin.GetNewSession();
      session.ExecuteNonQuery("DROP TABLE Test");
    }
    catch { }

    this.plugin.Dispose();
    this.session.Dispose();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public void NHibernate_Insert()
  {
    // using var session = plugin.GetNewSession();
    repo.Insert(session, repo.Create(new Dictionary<string, object>() { { "Id", 1 }, { "Name", "Test" } }));
  }

  [Fact]
  public void NHibernate_FindAll()
  {
    // using var session = plugin.GetNewSession();
    dynamic newObj = repo.Insert(session, repo.Create(new Dictionary<string, object>() { { "Id", 2 }, { "Name", "Test" } }));

    var all = repo.FindAll(session).Cast<dynamic>().ToList();
    Assert.NotEmpty(all);
    Assert.Equal(2, all[0].Id);
    Assert.Equal("Test", all[0].Name);
  }
}
