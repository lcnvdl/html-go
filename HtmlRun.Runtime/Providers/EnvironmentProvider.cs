using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

public class EnvironmentProvider : INativeProvider
{
  public string Namespace => "Environment";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new GetArgsCmd(), new GetCurrentDirectoryCmd(), };
}

class GetArgsCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.EnvironmentInstructionsSet.GetArgs;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string[]>(() => Environment.GetCommandLineArgs());
  }
}

class GetCurrentDirectoryCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.EnvironmentInstructionsSet.CurrentDirectory;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string>(() => Environment.CurrentDirectory);
  }
}