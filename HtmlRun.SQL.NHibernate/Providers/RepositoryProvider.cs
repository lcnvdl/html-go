using System.Dynamic;
using System.Text.Json;
using HtmlRun.Common.Models;
using HtmlRun.Common.Plugins.SQL;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Runtime.Utils;
using HtmlRun.SQL.NHibernate.Utils;

namespace HtmlRun.SQL.NHibernate.Providers;

public class RepositoryProvider : INativeProvider
{
  public string Namespace => "Repository";

  public INativeInstruction[] Instructions => new INativeInstruction[]
  {
    new GetListCmd(),
    new FindOneCmd(),
    new FindAllCmd(),
    new SaveEntityCmd(),
  };
}

class GetListCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "GetList";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>((entity) =>
    {
      IEntityRepository repo = Plugin.Instance.GetRepository(entity);
      using var session = Plugin.Instance.GetNewSession();

      List<ExpandoObject> entities = repo.FindAll(session).ToList();

      return JsonSerializer.Serialize(entities);
    });
  }
}

class FindOneCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "FindOne";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, Jurassic.Library.ObjectInstance, string>((entityName, where) =>
    {
      IEntityRepository repo = Plugin.Instance.GetRepository(entityName);
      using var session = Plugin.Instance.GetNewSession();

      ExpandoObject? entity;

      if (where != null)
      {
        entity = repo.Find(session, JurassicUtils.ToDictionary(where));
      }
      else
      {
        throw new NotImplementedException();
      }

      return JsonSerializer.Serialize(entity);
    });
  }
}


class FindAllCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => "FindAll";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, Jurassic.Library.ObjectInstance, string>((entityName, where) =>
    {
      IEntityRepository repo = Plugin.Instance.GetRepository(entityName);
      using var session = Plugin.Instance.GetNewSession();

      List<ExpandoObject> entities;

      if (where != null)
      {
        entities = repo.FindAll(session, JurassicUtils.ToDictionary(where)).ToList();
      }
      else
      {
        entities = repo.FindAll(session).ToList();
      }

      return JsonSerializer.Serialize(entities);
    });
  }
}

class SaveEntityCmd : INativeInstruction
{
  public string Key => "Save";

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string varNameOfInstanceToSave = ctx.GetRequiredArgument();

        ContextValue instanceVariable = this.GetAndValidateEntityInstanceVariable(ctx, varNameOfInstanceToSave);

        EntityModel entityModel = this.GetEntityModel(ctx, int.Parse(instanceVariable.Value!), varNameOfInstanceToSave);

        // Fill dictionaries with in-memory values

        Dictionary<string, object?> valuesFromInstance = this.AttributeValuesFromContextToInstance(ctx, varNameOfInstanceToSave, entityModel);
        Dictionary<string, object> notNullValuesFromInstance = new();

        foreach (var kv in valuesFromInstance)
        {
          if (kv.Value != null)
          {
            notNullValuesFromInstance[kv.Key] = kv.Value;
          }
        }

        //  Save

        string realEntityName = entityModel.Name;

        var repo = Plugin.Instance.GetRepository(realEntityName);

        Plugin.Instance.RunAndCommitTransaction((tx, session) =>
        {
          Dictionary<string, object?>? finalEntity = null;

          bool isNewEntity = true;

          //  Fill finalEntity with existing
          if (repo.SatisfiesPK(notNullValuesFromInstance))
          {
            var existingExpando = repo.Find(session, notNullValuesFromInstance);
            if (existingExpando != null)
            {
              isNewEntity = false;

              finalEntity = new Dictionary<string, object?>(existingExpando);
            }
          }

          //  If it wasn't filled, fill with defaults
          if (finalEntity == null)
          {
            finalEntity = new Dictionary<string, object?>(repo.Create(valuesFromInstance));
          }
          else
          {
            //  Else, fill with in-memory values
            foreach (var kv in valuesFromInstance)
            {
              finalEntity[kv.Key] = kv.Value;
            }
          }

          var finalExpando = ExpandoUtils.ToExpandoWithNullableValues(finalEntity);

          //  Final save
          if (isNewEntity)
          {
            repo.Insert(session, finalExpando);
          }
          else
          {
            repo.Update(session, finalExpando);
          }
        });

      };
    }
  }

  private Dictionary<string, object?> AttributeValuesFromContextToInstance(ICurrentInstructionContext ctx, string varNameOfInstanceInMemory, EntityModel model)
  {
    Dictionary<string, object?> valuesFromInstance = new();

    foreach (var attribute in model.Attributes)
    {
      string attributeNameInContext = $"{varNameOfInstanceInMemory}.{attribute.Name}";
      var savedValueVariable = ctx.GetVariable(attributeNameInContext);

      if (savedValueVariable != null)
      {
        valuesFromInstance[attribute.Name] = (savedValueVariable.Value == null) ? null : SqlUtils.SqlCast(savedValueVariable.Value, attribute.SqlType);
      }
    }

    return valuesFromInstance;
  }

  private EntityModel GetEntityModel(ICurrentInstructionContext ctx, int pointer, string instanceName)
  {
    return ctx.PointerToEntity(pointer) ?? throw new Exception($"Entity referenced by instance {instanceName} cannot be found.");
  }

  private ContextValue GetAndValidateEntityInstanceVariable(ICurrentInstructionContext ctx, string varNameOfEntityToSave)
  {
    var instanceVariable = ctx.GetVariable(varNameOfEntityToSave) ?? throw new Exception($"Entity instance {varNameOfEntityToSave} not found.");

    if (instanceVariable.IsUnset || string.IsNullOrEmpty(instanceVariable.Value))
    {
      throw new Exception($"The value {instanceVariable.Value} referenced by instance {varNameOfEntityToSave} is not an entity.");
    }

    return instanceVariable;
  }
}
