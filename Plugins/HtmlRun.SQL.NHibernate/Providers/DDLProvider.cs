using System.Text;
using System.Text.Json;
using FluentNHibernate.Cfg.Db;
using HtmlRun.Common.Models;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.SQL.NHibernate.Extensions;
using HtmlRun.SQL.NHibernate.Factories;
using HtmlRun.SQL.NHibernate.Utils;

using NH = NHibernate;

namespace HtmlRun.SQL.NHibernate.Providers;

public class DDLProvider : INativeProvider
{
  public string Namespace => "DDL";

  public INativeInstruction[] Instructions => new INativeInstruction[]
  {
    new EnsureEntityTableCmd(),
  };
}

public class TableNotFoundException : Exception
{
  public TableNotFoundException(string msg) : base(msg)
  {
  }
}

class EnsureEntityTableCmd : INativeInstruction
{
  public string Key => "EnsureEntityTable";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        using var session = Plugin.Instance.GetNewSession();
        string tableName = ctx.GetRequiredArgument();

        try
        {
          //  TODO  Hacer insert y rollback
          //  session.ExecuteSQLQueryAndGetUniqueResult($"SELECT COUNT(*) FROM {tableName}");

          var metadata = new NH.Tool.hbm2ddl.DatabaseMetadata(((SessionWrapper)session).NativeSession.Connection, GetDialect(Plugin.Instance.Settings));
          if (!metadata.IsTable(tableName))
          {
            throw new TableNotFoundException($"Table {tableName} not found.");
          }
        }
        catch (TableNotFoundException)
        {
          string entityJson = ctx.GetVariable($"Entities.{tableName}")?.Value ?? throw new Exception($"Entity {tableName} not found.");
          EntityModel entity = JsonSerializer.Deserialize<EntityModel>(entityJson) ?? throw new NullReferenceException();

          var sb = new StringBuilder();
          sb.AppendFormat("CREATE TABLE {0} (", tableName);

          bool isFirstAttribute = true;

          foreach (var attribute in entity.Attributes)
          {
            if (!isFirstAttribute)
            {
              sb.Append(',');
            }
            else
            {
              isFirstAttribute = false;
            }

            sb.AppendFormat("{0} {1} {2} {3}",
              attribute.Name,
              attribute.SqlTypeWithLength,
              attribute.IsNull ? "NULL" : "NOT NULL",
              attribute.IsPK ? "PRIMARY KEY" : "");
          }

          sb.Append(')');

          session.ExecuteNonQuery(sb.ToString());

          // new DatabaseMetadata()

          // ((SessionWrapper)session).NativeSession.
          // ((SessionWrapper)session).NativeSession.Connection
        }
      };
    }
  }

  private static NH.Dialect.Dialect GetDialect(PluginSettings settings)
  {
    NH.Dialect.Dialect dbSettings;

    switch (settings.DatabaseLibrary)
    {
      case Constants.DatabaseEngines.SQLServer:
        {
          dbSettings = new NH.Dialect.MsSql2008Dialect();
        }
        break;
      case Constants.DatabaseEngines.MySQL:
        {
          dbSettings = new NH.Dialect.MySQLDialect();
        }
        break;
      case Constants.DatabaseEngines.Oracle:
        {
          dbSettings = settings.DatabaseLibraryVersion == "9" ? new NH.Dialect.Oracle9iDialect() : new NH.Dialect.Oracle10gDialect();
        }
        break;
      case Constants.DatabaseEngines.SQLite:
        {
          dbSettings = new NH.Dialect.SQLiteDialect();
        }
        break;

      default:
        dbSettings = new NH.Dialect.SQLiteDialect();
        break;
    }

    return dbSettings;
  }
}