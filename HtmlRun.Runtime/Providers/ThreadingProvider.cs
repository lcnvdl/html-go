using System.Reflection;
using HtmlRun.Runtime.Exceptions;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class ThreadingProvider : INativeProvider
{
  public string Namespace => "Threading";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new SleepCmd(), new IncrementCmd(), new DecrementCmd(), };
}

class SleepCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.ThreadingInstructionsSet.Sleep;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Thread.Sleep(ctx.GetRequiredArgument<int>());
    }
  }

  public Delegate ToJSAction()
  {
    return new Action<int>(Thread.Sleep);
  }
}

class IncrementCmd : INativeInstruction
{
  public string Key => Constants.ThreadingInstructionsSet.Increment;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string varName = ctx.GetRequiredArgument();
        var meta = ctx.GetVariable(varName);

        if (meta == null)
        {
          throw new VariableNotFoundException(varName);
        }

        var cast = Utils.CastingUtils.ToNumber(meta.Value);
        
        if (cast == null)
        {
          throw new InvalidCastException($"Variable {varName} is not a number.");
        }

        object finalValue = InterlokedUtils.Call("Increment", cast.Value);

        ctx.SetValueVariable(varName, finalValue.ToString()!);
      };
    }
  }
}

class DecrementCmd : INativeInstruction
{
  public string Key => Constants.ThreadingInstructionsSet.Decrement;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string varName = ctx.GetRequiredArgument();
        var meta = ctx.GetVariable(varName);

        if (meta == null)
        {
          throw new VariableNotFoundException(varName);
        }

        var cast = Utils.CastingUtils.ToNumber(meta.Value);
        
        if (cast == null)
        {
          throw new InvalidCastException($"Variable {varName} is not a number.");
        }

        object finalValue = InterlokedUtils.Call("Decrement", cast.Value);

        ctx.SetValueVariable(varName, finalValue.ToString()!);
      };
    }
  }
}

static class InterlokedUtils
{
  internal static object Call(string method, object value)
  {
    Type t = value.GetType();
    return typeof(Interlocked).GetMethods(BindingFlags.Public | BindingFlags.Static)
      .First(m => m.Name.Equals(method) && m.ReturnType == t)
      .Invoke(null, new[] { value })!;
  }
}