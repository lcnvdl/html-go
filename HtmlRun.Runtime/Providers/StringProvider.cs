using System.Text.Json;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class StringProvider : INativeProvider
{
  public string Namespace => "String";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new ConcatCmd(),
    new JoinCmd(),
    new SplitCmd(),
    new TrimCmd(),
    new ToLowerCaseCmd(),
    new ToTitleCaseCmd(),
    new ToUpperCaseCmd(),
  };
}

class ConcatCmd : INativeInstruction, INativeJSEvalInstruction
{
  public string Key => Constants.StringInstructionsSet.Concat;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public EvalDefinition ToEvalFunction()
  {
    return new EvalDefinition("return [args].join('');", -1);
  }
}


class JoinCmd : INativeInstruction, INativeJSEvalInstruction
{
  public string Key => Constants.StringInstructionsSet.Join;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public EvalDefinition ToEvalFunction()
  {
    return new EvalDefinition("return arg1.join(arg0);", 2);
  }
}

class TrimCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.StringInstructionsSet.Trim;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(arg => arg.Trim());
  }
}

class ToUpperCaseCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.StringInstructionsSet.ToUpperCase;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(arg => arg.ToUpper());
  }
}

class ToLowerCaseCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.StringInstructionsSet.ToLowerCase;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(arg => arg.ToLower());
  }
}

class ToTitleCaseCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.StringInstructionsSet.ToTitleCase;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string>(arg => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(arg));
  }
}

class SplitCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.StringInstructionsSet.Split;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }
  public Delegate ToJSAction()
  {
    var jsDelegate = new Func<string, string, string>((text, separator) =>
    {
      string[] split = text.Split(separator);
      return JsonSerializer.Serialize(split);
    });

    return jsDelegate;
  }
}


