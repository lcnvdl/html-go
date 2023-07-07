using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class StringProvider : INativeProvider
{
  public string Namespace => "String";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new TrimCmd(), new ToUpperCaseCmd(), new ToLowerCaseCmd(), new ToTitleCaseCmd(), };
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


