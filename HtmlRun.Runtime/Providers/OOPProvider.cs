using System.Text.Json;
using HtmlRun.Common.Models;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class OOPProvider : INativeProvider
{
  public string Namespace => Constants.Namespaces.Global;

  public INativeInstruction[] Instructions => new INativeInstruction[] { new NewCmd() };
}

class NewCmd : INativeInstruction
{
  public string Key => Constants.BasicInstructionsSet.New;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        //  TODO  This should create pointer variables.
        //  TODO  This should be compatible with any object type (not only-entities).

        string entityType = ctx.GetRequiredArgument();
        string entityVariableName = ctx.GetRequiredArgument(1);

        var entityVariable = ctx.GetVariable(entityType);

        if (entityVariable == null || entityVariable.IsUnset || !entityVariable.IsConst)
        {
          throw new InvalidOperationException($"The variable {ctx.GetRequiredArgument()} is not a valid entity.");
        }

        //  Read the definition as JSON

        var entity = JsonSerializer.Deserialize<EntityModel>(entityVariable.Value!);

        if (entity == null)
        {
          throw new InvalidCastException();
        }

        //  Save entity as variable

        ctx.DeclareVariable(entityVariableName);
        ctx.AllocAndSetPointerVariable(entityVariableName, entity);
      };
    }
  }
}
