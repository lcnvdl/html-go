using System.Dynamic;

namespace HtmlRun.Common.Plugins.SQL;

public interface IEntityRepository
{
  ExpandoObject Create(IDictionary<string, object>? dictionary = null);

  ExpandoObject? Find(ISessionWrapper sessionWrapper, Dictionary<string, object> where);

  IEnumerable<ExpandoObject> FindAll(ISessionWrapper sessionWrapper);

  ExpandoObject Insert(ISessionWrapper sessionWrapper, ExpandoObject obj);

  void Update(ISessionWrapper sessionWrapper, ExpandoObject obj);

  ExpandoObject UpdateSet(ISessionWrapper sessionWrapper, ExpandoObject obj, Dictionary<string, object?> newValues);

  ExpandoObject Save(ISessionWrapper sessionWrapper, ExpandoObject entity);

  bool Exists(ISessionWrapper sessionWrapper, Dictionary<string, object> where) => this.Find(sessionWrapper, where) != null;
}