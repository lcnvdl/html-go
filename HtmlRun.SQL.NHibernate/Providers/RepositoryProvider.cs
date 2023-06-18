using System.Text.Json;
using HtmlRun.Common.Models;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.SQL.NHibernate.Extensions;
using HtmlRun.SQL.NHibernate.Utils;

namespace HtmlRun.SQL.NHibernate.Providers;

public class RepositoryProvider : INativeProvider
{
  public string Namespace => "Repository";

  public INativeInstruction[] Instructions => new INativeInstruction[]
  {
    new SaveEntityCmd(),
  };
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

        EntityModel entityModel = this.GetEntityModel(ctx, instanceVariable.Value!, varNameOfInstanceToSave);

        // Fill dictionaries with in-memory values

        Dictionary<string, object> notNullValuesFromInstance = new();
        Dictionary<string, object?> valuesFromInstance = new();

        foreach (var attribute in entityModel.Attributes)
        {
          string attributeNameInContext = $"{varNameOfInstanceToSave}.{attribute.Name}";
          var savedValueVariable = ctx.GetVariable(attributeNameInContext);

          if (savedValueVariable != null)
          {
            valuesFromInstance[attribute.Name] = (savedValueVariable.Value == null) ? null : SqlUtils.SqlCast(savedValueVariable.Value, attribute.SqlType);

            if (valuesFromInstance[attribute.Name] != null)
            {
              notNullValuesFromInstance[attribute.Name] = valuesFromInstance[attribute.Name]!;
            }
          }
        }

        //  Save

        string realEntityName = instanceVariable.Value!.StartsWith("Entities.") ?
          instanceVariable.Value!.Substring("Entities.".Length) : instanceVariable.Value!;

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

  private EntityModel GetEntityModel(ICurrentInstructionContext ctx, string entityName, string instanceName)
  {
    ContextValue entityVariable = ctx.GetVariable(entityName) ??
     throw new Exception($"Entity {entityName} referenced by instance {instanceName} cannot be found.");

    EntityModel entity = JsonSerializer.Deserialize<EntityModel>(entityVariable.Value!) ?? throw new NullReferenceException();

    return entity;
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
