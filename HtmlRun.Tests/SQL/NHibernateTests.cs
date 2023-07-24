using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.SQL.NHibernate;
using HtmlRun.SQL.NHibernate.Extensions;

public class NHibernateTests : IDisposable
{
  private Plugin plugin;

  private IEntityRepository repo;

  private ISessionWrapper session;

  private static EntityModel TestEntity => new ()
  {
    Name = "Test",
    Attributes = new List<EntityAttributeModel>()
      {
        new EntityAttributeModel() { Name = "Id", SqlType = "INTEGER", IsNull = false, IsPK = true },
        new EntityAttributeModel() { Name = "Name", SqlType = "TEXT", IsNull = false },
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
    repo.Insert(session, repo.Create(new Dictionary<string, object?>() { { "Id", 1 }, { "Name", "Test" } }));
  }

  [Fact]
  public void NHibernate_Update()
  {
    // using var session = plugin.GetNewSession();
    dynamic newEntity = repo.Insert(session, repo.Create(new Dictionary<string, object?>() { { "Id", 5 }, { "Name", "Tst" } }));
    newEntity.Name = "Test";
    repo.Update(session, newEntity);

    dynamic? entityAfterUpdate = repo.FindByPK(session, new Dictionary<string, object>() { { "Id", 5 } });
    Assert.NotNull(entityAfterUpdate);
    Assert.Equal("Test", entityAfterUpdate!.Name);
  }

  [Fact]
  public void NHibernate_UpdateSet()
  {
    // using var session = plugin.GetNewSession();
    dynamic original = repo.Insert(session, repo.Create(new Dictionary<string, object?>() { { "Id", 6 }, { "Name", "Tst" } }));
    dynamic final = repo.UpdateSet(session, original, new Dictionary<string, object?>() { { "Id", 10 }, { "Name", "Onix" } });

    Assert.NotNull(final);
    Assert.Equal(10, final.Id);
    Assert.Equal("Onix", final.Name);

    dynamic? oldEntityAfterUpdate = repo.FindByPK(session, new Dictionary<string, object>() { { "Id", 5 } });
    Assert.Null(oldEntityAfterUpdate);

    dynamic? entityAfterUpdate = repo.FindByPK(session, new Dictionary<string, object>() { { "Id", 10 } });
    Assert.NotNull(entityAfterUpdate);
    Assert.Equal(10, entityAfterUpdate!.Id);
    Assert.Equal("Onix", entityAfterUpdate!.Name);
  }

  [Fact]
  public void NHibernate_Find_ShouldReturnNullIfEntityDoesNotExists()
  {
    dynamic? entity = repo.FindByPK(session, new Dictionary<string, object>() { { "Id", 99 } });
    Assert.Null(entity);
  }

  [Fact]
  public void NHibernate_Find_ShouldWorkFine()
  {
    repo.Insert(session, repo.Create(new Dictionary<string, object?>() { { "Id", 6 }, { "Name", "Seis" } }));
    dynamic? entity = repo.FindByPK(session, new Dictionary<string, object>() { { "Id", 6 } });
    Assert.NotNull(entity);
    Assert.Equal(6, entity!.Id);
    Assert.Equal("Seis", entity!.Name);
  }

  [Fact]
  public void NHibernate_FindAll()
  {
    repo.Insert(session, repo.Create(new Dictionary<string, object?>() { { "Id", 2 }, { "Name", "Test" } }));

    var all = repo.FindAll(session).Cast<dynamic>().ToList();
    Assert.NotEmpty(all);
    Assert.Equal(2, all[0].Id);
    Assert.Equal("Test", all[0].Name);
  }

  [Fact]
  public void NHibernate_FindAll_WithWhere()
  {
    repo.Insert(session, repo.Create(new Dictionary<string, object?>() { { "Id", 3 }, { "Name", "Test" } }));

    var all = repo.FindAll(session, new Dictionary<string, object?>() { { "Id", 3 } }).Cast<dynamic>().ToList();
    Assert.NotEmpty(all);
    Assert.Equal(3, all[0].Id);
    Assert.Equal("Test", all[0].Name);
  }
}
