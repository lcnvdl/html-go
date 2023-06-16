using System.Data;
using System.Data.Common;
using System.Dynamic;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.SQL.NHibernate.Extensions;
using HtmlRun.SQL.NHibernate.Utils;
using NHibernate;

namespace HtmlRun.SQL.NHibernate;

public class EntityRepository
{
  public EntityModel Entity { get; private set; }

  public EntityRepository(EntityModel model)
  {
    this.Entity = model;
  }

  private IEnumerable<string> AttributeNames => this.Entity.Attributes.Select(m => m.Name);

  private string MergedAttributeNames => string.Join(',', this.AttributeNames);

  private IDictionary<string, object> Defaults
  {
    get
    {
      var defaults = new Dictionary<string, object>();

      foreach (var attribute in this.Entity.Attributes)
      {
        if (attribute.DefaultValue != null)
        {
          defaults[attribute.Name] = SqlUtils.SqlCast(attribute.DefaultValue, attribute.SqlType);
        }
      }

      return defaults;
    }
  }

  public ExpandoObject Create(IDictionary<string, object>? dictionary = null)
  {
    dynamic obj = ExpandoUtils.ToExpando(dictionary ?? this.Defaults);
    return obj;
  }

  public ExpandoObject Insert(ISessionWrapper sessionWrapper, ExpandoObject obj)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;
    var dictionary = new Dictionary<string, object?>(obj);

    string keys = this.MergedAttributeNames;
    string paramNames = string.Join(',', this.Entity.Attributes.Select(m => $":{m.Name}"));

    string query = $"INSERT INTO {this.Entity.Name} ({keys}) VALUES ({paramNames})";

    var command = session.Connection.CreateCommand();
    command.CommandText = query;

    for (int i = 0; i < this.Entity.Attributes.Count; i++)
    {
      var parameter = command.CreateParameter();
      parameter.ParameterName = this.Entity.Attributes[i].Name;
      parameter.Value = dictionary[this.Entity.Attributes[i].Name];
      command.Parameters.Add(parameter);
    }

    session.Enlist(command);

    command.ExecuteNonQuery();

    return obj;
  }

  public IEnumerable<ExpandoObject> FindAll(ISessionWrapper sessionWrapper)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;
    var query = session.CreateSQLQuery($"SELECT {MergedAttributeNames} FROM {this.Entity.Name}");

    var list = query.List();


    var rows = list.Cast<object[]>().ToArray();
    var names = this.AttributeNames.ToArray();

    foreach (var row in rows)
    {
      var entity = new Dictionary<string, object>();
      
      for (int i = 0; i < names.Length; i++)
      {
        entity[names[i]] = row[i];
      }

      yield return ExpandoUtils.ToExpando(entity);
    }
  }

  public void Update(ISessionWrapper sessionWrapper, ExpandoObject obj)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;
    var dictionary = new Dictionary<string, object?>(obj);
    string keys = string.Join(',', this.Entity.Attributes.Select(m => m.Name));
    string values = string.Join(',', dictionary.Keys.Cast<string>().Select(m => "?"));

    string query = $"UPDATE {this.Entity.Name} ({keys}) VALUES ({values})";

    var insert = session.CreateSQLQuery(query);

    for (int i = 0; i < this.Entity.Attributes.Count; i++)
    {
      insert.SetParameter(i, dictionary[this.Entity.Attributes[i].Name]);
    }

    insert.ExecuteUpdate();
  }
}

