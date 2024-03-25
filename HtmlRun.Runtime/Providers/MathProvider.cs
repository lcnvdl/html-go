using System.Globalization;
using HtmlRun.Runtime.Exceptions;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class MathProvider : INativeProvider
{
  public string Namespace => "Math";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new ClampCmd(),
    new IncrementValueCmd(),
    new DecrementValueCmd(),
  };
}

class ClampCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.MathInstructionsSet.Clamp;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<double, double, double, double>((value, min, max) => Math.Clamp(value, min, max));
  }
}

class IncrementValueCmd : INativeInstruction, INativeJSEvalInstruction
{
  public string Key => Constants.MathInstructionsSet.IncrementValue;

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

        object finalValue = MathUtils.Increment(cast.Value);

        ctx.SetValueVariable(varName, finalValue.ToString()!);
      };
    }
  }

  public EvalDefinition ToEvalFunction()
  {
    return new EvalDefinition("return (+arg0) + 1;", 1);
  }
}

class DecrementValueCmd : INativeInstruction, INativeJSEvalInstruction
{
  public string Key => Constants.MathInstructionsSet.DecrementValue;

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

        object finalValue = MathUtils.Decrement(cast.Value);

        ctx.SetValueVariable(varName, finalValue.ToString()!);
      };
    }
  }

  public EvalDefinition ToEvalFunction()
  {
    return new EvalDefinition("return (+arg0) - 1;", 1);
  }
}

internal static class MathUtils
{
  internal static object Decrement(object value)
  {
    return Parse(value, -1);
  }

  internal static object Increment(object value)
  {
    return Parse(value, 1);
  }

  private static object Parse(object value, int v)
  {
    if (value is Int32)
    {
      return ((Int32)value) + v;
    }

    if (value is Int16)
    {
      return ((Int16)value) + v;
    }

    if (value is Int64)
    {
      return ((Int64)value) + v;
    }

    if (value is float)
    {
      return ((float)value) + v;
    }

    if (value is double)
    {
      return ((double)value) + v;
    }

    if (value is decimal)
    {
      return ((decimal)value) + v;
    }

    string valueAsString = value.ToString() ?? string.Empty;

    if (valueAsString.Contains(','))
    {
      valueAsString = valueAsString.Replace(',', '.');
    }

    if (valueAsString.Contains('.'))
    {
      return decimal.Parse(valueAsString, CultureInfo.InvariantCulture) + v;
    }

    return int.Parse(valueAsString) + v;
  }
}