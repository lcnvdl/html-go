using System.Collections;
using System.Text.Json;
using HtmlRun.Runtime.Exceptions;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class ListProvider : INativeProvider
{
  public string Namespace => "List";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new ListGetCmd(),
    new ListNewCmd(),
    new ListAddCmd(),
    new ListSortCmd(),
    new ListRemoveCmd(),
    new ListRemoveAtCmd(),
    new ListSwapCmd(),
    new ListGetSizeCmd(),
  };
}

class ListGetCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.ListInstructionsSet.Get;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, int, string>((json, index) =>
    {
      var list = JsonSerializer.Deserialize<List<string>>(json);

      if (list == null)
      {
        throw new NullReferenceException($"List is not valid.");
      }

      return list[index];
    });
  }
}

class ListGetSizeCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.ListInstructionsSet.GetSize;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, int>((json) =>
    {
      var list = JsonSerializer.Deserialize<ArrayList>(json);

      if (list == null)
      {
        throw new NullReferenceException($"List is not valid.");
      }

      return list.Count;
    });
  }
}

class ListNewCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.ListInstructionsSet.New;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string>(() => "[]");
  }
}

abstract class ListManipulationBaseCmd
{
  protected List<string> GetList(ICurrentInstructionContext ctx)
  {
    string jsonVariableName = ctx.GetRequiredArgument(0);

    var jsonVariable = ctx.GetVariable(jsonVariableName) ?? throw new VariableNotFoundException(ctx.GetRequiredArgument(0));
    var json = jsonVariable.Value;

    if (string.IsNullOrEmpty(json))
    {
      throw new InvalidCastException($"The value of {jsonVariableName} is not a list.");
    }

    var list = JsonSerializer.Deserialize<List<string>>(json);

    if (list == null)
    {
      throw new InvalidCastException($"The value of {jsonVariableName} is not a list.");
    }

    return list;
  }

  protected void SaveList(ICurrentInstructionContext ctx, List<string> list)
  {
    ctx.SetValueVariable(ctx.GetRequiredArgument(0), JsonSerializer.Serialize(list));
  }
}

class ListAddCmd : ListManipulationBaseCmd, INativeInstruction
{
  public string Key => Constants.ListInstructionsSet.Add;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string newItem = ctx.GetRequiredArgument(1);

        var list = base.GetList(ctx);

        list.Add(newItem);

        base.SaveList(ctx, list);
      };
    }
  }
}

class ListRemoveCmd : ListManipulationBaseCmd, INativeInstruction
{
  public string Key => Constants.ListInstructionsSet.Remove;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string itemValue = ctx.GetRequiredArgument(1);

        var list = base.GetList(ctx);

        list.Remove(itemValue);

        base.SaveList(ctx, list);
      };
    }
  }
}

class ListSwapCmd : ListManipulationBaseCmd, INativeInstruction
{
  public string Key => Constants.ListInstructionsSet.Swap;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        int index1 = int.Parse(ctx.GetRequiredArgument(1));
        int index2 = int.Parse(ctx.GetRequiredArgument(2));

        var list = base.GetList(ctx);

        var aux = list[index1];
        list[index1] = list[index2];
        list[index2] = aux;

        base.SaveList(ctx, list);
      };
    }
  }
}

class ListRemoveAtCmd : ListManipulationBaseCmd, INativeInstruction
{
  public string Key => Constants.ListInstructionsSet.RemoveAt;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        int index = int.Parse(ctx.GetRequiredArgument(1));

        var list = base.GetList(ctx);

        list.RemoveAt(index);

        base.SaveList(ctx, list);
      };
    }
  }
}

class ListSortCmd : ListManipulationBaseCmd, INativeInstruction
{
  public string Key => Constants.ListInstructionsSet.Sort;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        var list = base.GetList(ctx);

        list.Sort();

        base.SaveList(ctx, list);
      };
    }
  }
}
