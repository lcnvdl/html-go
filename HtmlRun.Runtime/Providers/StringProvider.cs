using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using Jurassic.Library;

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


class JoinCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.StringInstructionsSet.Join;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<ArrayInstance, string, string>((arr, separator) => string.Join(separator, arr.ElementValues));
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

class SplitCmd : INativeInstruction, INativeJSInstruction, IInstructionRequiresJsEngine
{
  private Func<JavascriptParserWithContext>? engineGetter;

  public string Key => Constants.StringInstructionsSet.Split;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public void SetEngineGenerator(Func<object> engineGetter)
  {
    this.engineGetter = () => (JavascriptParserWithContext)engineGetter();
  }

  public Delegate ToJSAction()
  {
    var jsDelegate = new Func<string, string, ArrayInstance>((text, separator) =>
    {
      string[] split = text.Split(separator);

      if (this.engineGetter == null)
      {
        throw new NullReferenceException("Missing engine generator.");
      }

      var arr = (ArrayInstance)this.engineGetter().GetInteropArray(split);
      return arr;
    });

    return jsDelegate;
  }
}


