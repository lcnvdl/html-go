using HtmlRun.Common.Models;
using HtmlRun.Runtime;
using HtmlRun.SQL.NHibernate;

public class NHibernateTests
{
  [Fact]
  public void NHibernate_GeneralTest()
  {
    Plugin plugin = new(GetType().Assembly);

    plugin.BeforeApplicationLoad(new HtmlRun.Common.Plugins.Models.ApplicationStartEventModel(Environment.GetEnvironmentVariables()));

    plugin.OnLoadEntity(new EntityModel()
    {
      Name = "Test",
      Attributes = new List<EntityAttributeModel>()
      {
        new EntityAttributeModel() { Name = "Id", SqlType = "INTEGER"},
        new EntityAttributeModel() { Name = "Name", SqlType = "TEXT"},
      }
    });

    plugin.OnApplicationStart(new HtmlRun.Common.Plugins.Models.ApplicationStartEventModel(Environment.GetEnvironmentVariables()));

    var repo = plugin.GetRepository("Test");

    using var session = plugin.GetNewSession();

    repo.RunNoSQL(session, "CREATE TABLE Test ( Id INTEGER PRIMARY KEY, Name TEXT )");

    repo.Insert(session, repo.Create(new Dictionary<string, object>() { { "Id", 1 }, { "Name", "Test" } }));
  }
}