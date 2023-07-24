using System.Collections;
using System.Data;
using System.Dynamic;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.SQL.NHibernate.Extensions;
using HtmlRun.SQL.NHibernate.Utils;
using NHibernate;

namespace HtmlRun.SQL.NHibernate;

public class EntityRepository : IEntityRepository
{
  private EntityModel Entity { get; set; }

  public EntityRepository(EntityModel model)
  {
    this.Entity = model;
  }

  private IEnumerable<EntityAttributeModel> PKs => this.Entity.Attributes.Where(m => m.IsPK);

  private IEnumerable<string> PKNames => this.PKs.Select(m => m.Name);

  private IEnumerable<string> AttributeNames => this.Entity.Attributes.Select(m => m.Name);

  private string MergedAttributeNames => string.Join(',', this.AttributeNames);

  private IDictionary<string, object?> Defaults
  {
    get
    {
      var defaults = new Dictionary<string, object?>();

      foreach (var attribute in this.Entity.Attributes)
      {
        if (attribute.DefaultValue != null)
        {
          defaults[attribute.Name] = SqlUtils.SqlCast(attribute.DefaultValue, attribute.SqlType);
        }
        else if (attribute.IsNull)
        {
          defaults[attribute.Name] = null;
        }
      }

      return defaults;
    }
  }

  public ExpandoObject Create(IDictionary<string, object?>? dictionary = null)
  {
    dynamic obj = ExpandoUtils.ToExpandoWithNullableValues(dictionary ?? this.Defaults);
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

  public ExpandoObject Save(ISessionWrapper sessionWrapper, ExpandoObject entity)
  {
    Dictionary<string, object?> entityAsDict = new(entity);
    var pks = this.GetValuesFromObjectPKs(entityAsDict);

    var existing = this.FindByPK(sessionWrapper, pks);

    if (existing == null)
    {
      return this.Insert(sessionWrapper, entity);
    }

    return this.UpdateSet(sessionWrapper, existing, entityAsDict);
  }

  public ExpandoObject? FindByPK(ISessionWrapper sessionWrapper, Dictionary<string, object> where)
  {
    return this.Find(sessionWrapper, where.ToDictionary(m => m.Key, m => (object?)m.Value));
  }

  public ExpandoObject? Find(ISessionWrapper sessionWrapper, Dictionary<string, object?> where)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;
    var query = session.CreateSQLQuery(SqlUtils.SelectLimitRows(
      sessionWrapper.DatabaseEngine,
      $"SELECT {this.MergedAttributeNames} FROM {this.Entity.Name} WHERE {this.DictionaryKeysToWhere(where)}",
      1
    ));

    for (int i = 0; i < this.Entity.Attributes.Count; i++)
    {
      string name = this.Entity.Attributes[i].Name;
      if (where.TryGetValue(name, out var value))
      {
        query.SetParameter(name, value);
      }
    }

    var row = query.List().Cast<object[]>().FirstOrDefault();

    if (row == null)
    {
      return null;
    }

    return this.RowToObject(row);
  }

  public IEnumerable<ExpandoObject> FindAll(ISessionWrapper sessionWrapper, Dictionary<string, object?>? where = null)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;

    string querySql = $"SELECT {this.MergedAttributeNames} FROM {this.Entity.Name}";

    if (where != null)
    {
      var ands = new List<string>();
      foreach (var kv in where)
      {
        ands.Add($"{kv.Key}=:{kv.Key}");
      }

      querySql += " WHERE (" + string.Join(" AND ", ands) + ")";
    }

    var query = session.CreateSQLQuery(querySql);

    if (where != null)
    {
      foreach (var kv in where)
      {
        query.SetParameter(kv.Key, kv.Value);
      }
    }

    var list = query.List();

    var rows = list.Cast<object[]>().ToArray();

    return this.RowsToObject(rows);
  }

  public ExpandoObject UpdateSet(ISessionWrapper sessionWrapper, ExpandoObject obj, Dictionary<string, object?> newValues)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;

    var originalValues = new Dictionary<string, object?>(obj);

    string setQuery = string.Join(',', newValues.Keys.Cast<string>().Select(m => $"{m}=:set{m}"));
    string where = this.DictionaryKeysToWhere(this.GetValuesFromObjectPKs(originalValues));
    string query = $"UPDATE {this.Entity.Name} SET {setQuery} WHERE {where}";

    var insert = session.CreateSQLQuery(query);

    for (int i = 0; i < this.Entity.Attributes.Count; i++)
    {
      var attr = this.Entity.Attributes[i];

      if (attr.IsPK)
      {
        insert.SetParameter(attr.Name, originalValues[attr.Name]);
      }

      if (newValues.ContainsKey(attr.Name))
      {
        insert.SetParameter($"set{attr.Name}", newValues[attr.Name]);
      }
    }

    insert.ExecuteUpdate();

    foreach (var kv in newValues)
    {
      originalValues[kv.Key] = kv.Value;
    }

    return ExpandoUtils.ToExpandoWithNullableValues(originalValues);
  }

  public void Update(ISessionWrapper sessionWrapper, ExpandoObject obj)
  {
    var session = ((SessionWrapper)sessionWrapper).NativeSession;
    var values = new Dictionary<string, object?>(obj);

    string setNoPksQuery = string.Join(',', values.Keys.Cast<string>().Except(this.PKNames).Select(m => $"{m}=:set{m}"));
    string where = this.DictionaryKeysToWhere(this.GetValuesFromObjectPKs(values));
    string query = $"UPDATE {this.Entity.Name} SET {setNoPksQuery} WHERE {where}";

    var insert = session.CreateSQLQuery(query);

    for (int i = 0; i < this.Entity.Attributes.Count; i++)
    {
      var attr = this.Entity.Attributes[i];
      if (attr.IsPK)
      {
        insert.SetParameter(attr.Name, values[attr.Name]);
      }
      else
      {
        insert.SetParameter($"set{attr.Name}", values[attr.Name]);
      }
    }

    insert.ExecuteUpdate();
  }

  private IEnumerable<ExpandoObject> RowsToObject(object[][] rows)
  {
    foreach (var row in rows)
    {
      yield return this.RowToObject(row);
    }
  }

  private ExpandoObject RowToObject(object[] row)
  {
    var names = this.AttributeNames.ToArray();

    var entity = new Dictionary<string, object>();

    for (int i = 0; i < names.Length; i++)
    {
      entity[names[i]] = row[i];
    }

    return ExpandoUtils.ToExpando(entity);
  }

  private Dictionary<string, object> GetValuesFromObjectPKs(Dictionary<string, object?> obj)
  {
    var result = new Dictionary<string, object>();

    foreach (var pk in this.PKNames)
    {
      object? value = obj[pk];

      if (value == null)
      {
        throw new QueryException($"The {pk} value for entity {this.Entity.Name} cannot be null.");
      }

      result[pk] = obj[pk]!;
    }

    return result;
  }

  private string DictionaryKeysToWhere(IDictionary objectId, string prefix = "")
  {
    return string.Join(" AND ", objectId.Keys.Cast<object>().Select(m => m.ToString()).Select(key => $"{key}=:{prefix}{key}"));
  }

  public bool SatisfiesPK(Dictionary<string, object> where)
  {
    return this.PKs.All(pk => where.ContainsKey(pk.Name) && where[pk.Name] != null);
  }
}

