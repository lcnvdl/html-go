using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

 class EnvironmentProvider : INativeProvider
{
  public string Namespace => "Environment";

  public INativeInstruction[] Instructions => new INativeInstruction[]
  {
    new GetArgsCmd(),
    new GetCurrentDirectoryCmd(),
    new GetEntryFileCmd(),
    new GetEntryDirectoryCmd(),
    new GetEnvironmentVariableCmd(),
    new SetEnvironmentVariableCmd(),
  };
}

class SetEnvironmentVariableCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.EnvironmentInstructionsSet.SetEnvironmentVariable;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => Environment.SetEnvironmentVariable(ctx.GetRequiredArgument(), ctx.GetRequiredArgument(1));
    }
  }

  public Delegate ToJSAction()
  {
    return new Action<string, string>((name, value) => Environment.SetEnvironmentVariable(name, value));
  }
}

class GetEnvironmentVariableCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.EnvironmentInstructionsSet.GetEnvironmentVariable;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string?>(name => Environment.GetEnvironmentVariable(name));
  }
}

class GetEntryFileCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.EnvironmentInstructionsSet.GetEntryFile;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string?>(name => Environment.GetEnvironmentVariable("ENTRY_FILE"));
  }
}

class GetEntryDirectoryCmd : INativeInstruction, INativeJSInstruction
{
  public string Key => Constants.EnvironmentInstructionsSet.GetEntryDirectory;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string, string?>(name => Environment.GetEnvironmentVariable("ENTRY_DIRECTORY"));
  }
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
