using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("HtmlRun.Tests")]

namespace HtmlRun.Runtime.Providers;

class StringProvider : INativeProvider
{
  public string Namespace => "String";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new TrimCmd(),
    new ToUpperCaseCmd(),
    new ToLowerCaseCmd(),
    new ToTitleCaseCmd(),
    new ConcatCmd(),
    new JoinCmd(),
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
    return new EvalDefinition("return [arg0||'',arg1||'',arg2||''].join('');", 3);
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
    // return new Func<string, string[], string>((separator, values) => string.Join(separator, values));
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


