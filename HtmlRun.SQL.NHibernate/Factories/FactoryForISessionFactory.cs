using System.Reflection;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace HtmlRun.SQL.NHibernate.Factories;

public static class FactoryForISessionFactory
{
  public static ISessionFactory CreateSessionFactory(PluginSettings settings)
  {
    var cfg = FluentNHibernate.Cfg.Fluently.Configure();

    IPersistenceConfigurer dbSettings;

    switch (settings.DatabaseLibrary)
    {
      case Constants.DatabaseEngines.SQLServer:
        {
          MsSqlConfiguration? partialDbSettings = string.IsNullOrEmpty(settings.DatabaseLibraryVersion) ?
            MsSqlConfiguration.MsSql2012 :
            (MsSqlConfiguration?)typeof(MsSqlConfiguration).GetProperties(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.CanRead && m.DeclaringType == typeof(MsSqlConfiguration) && m.Name == settings.DatabaseLibraryVersion)
            .GetValue(null);

          if (partialDbSettings == null)
          {
            throw new Exception($"SQLServer version {settings.DatabaseLibraryVersion} not found.");
          }

          partialDbSettings.ConnectionString(settings.ConnectionString);
          dbSettings = partialDbSettings;
        }
        break;
      case Constants.DatabaseEngines.MySQL:
        {
          var partialDbSettings = MySQLConfiguration.Standard;
          partialDbSettings.ConnectionString(settings.ConnectionString);
          dbSettings = partialDbSettings;
        }
        break;
      case Constants.DatabaseEngines.Oracle:
        {
          var partialDbSettings = settings.DatabaseLibraryVersion == "9" ? OracleDataClientConfiguration.Oracle9 : OracleDataClientConfiguration.Oracle10;
          partialDbSettings.ConnectionString(settings.ConnectionString);
          dbSettings = partialDbSettings;
        }
        break;
      case Constants.DatabaseEngines.SQLite:
        {
          var partialDbSettings = SQLiteConfiguration.Standard;

          if (string.IsNullOrEmpty(settings.ConnectionString))
          {
            partialDbSettings.InMemory();
          }
          else
          {
            partialDbSettings.ConnectionString(settings.ConnectionString);
          }

          dbSettings = partialDbSettings;
        }
        break;

      default:
        dbSettings = SQLiteConfiguration.Standard.InMemory().ShowSql();
        break;
    }

    // .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Person>())

    cfg.Database(dbSettings);

    return cfg.ExposeConfiguration(BuildSchema).BuildSessionFactory();

    /*var cfg = new Configuration().DataBaseIntegration(db =>
      {
        // db.ConnectionString = "FullUri=file:memorydb.db?mode=memory&cache=shared";
        db.ConnectionString = "Data Source=:memory:;Version=3;New=True;";
        db.Dialect<SQLiteDialect>();
        db.Driver<SQLite20Driver>();
        db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
        db.LogSqlInConsole = true;
        db.LogFormattedSql = true;
      });
    // .AddAssembly(this.AppAssembly);

    return cfg.BuildSessionFactory();*/
    // return Fluently.Configure().BuildSessionFactory();
  }

  private static void BuildSchema(Configuration cfg)
  {
    new SchemaExport(cfg).Create(true, true);
  }
}