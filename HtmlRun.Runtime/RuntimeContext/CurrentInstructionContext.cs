using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.RuntimeContext;

public class CurrentInstructionContext : BaseContext, ICurrentInstructionContext
{
  private Stack<Context> ctxStack;

  private string? callName;

  private List<string?>? args;

  public IRuntimeContext ParentContext { get; private set; }

  public IContextJump? CursorModification { get; set; }

  public ContextValue[] AllVariables => this.ParentContext.AllVariables;

  public List<string> DirtyVariables { get; private set; } = new List<string>();

  public CurrentInstructionContext(IRuntimeContext parent, Stack<Context> ctxStack, string callName, IEnumerable<string?> args)
  {
    this.ParentContext = parent;
    this.ctxStack = ctxStack;
    this.callName = callName;
    this.args = args.ToList();
  }

  public T GetRequiredArgument<T>(int idx = 0, string? errorMessage = null)
  {
    if (this.args == null)
    {
      throw new NullReferenceException();
    }

    if (this.args[idx] == null)
    {
      throw new ArgumentException(errorMessage ?? $"Argument {idx} is missing.");
    }

    var newType = Convert.ChangeType(this.args[idx], typeof(T));

    if (newType == null)
    {
      throw new InvalidCastException();
    }

    return (T)newType;
  }

  public T? GetArgument<T>(int idx = 0)
  {
    if (this.args == null)
    {
      throw new NullReferenceException();
    }

    if (this.args[idx] == null)
    {
      return default(T);
    }

    var newType = Convert.ChangeType(this.args[idx], typeof(T));

    if (newType == null)
    {
      return default(T);
    }

    return (T)newType;
  }

  public int CountArguments()
  {
    if (this.args == null)
    {
      return 0;
    }

    return this.args.Count;
  }

  public string?[] GetArguments()
  {
    return this.args?.ToArray() ?? new string?[] { };
  }

  public void Jump<T>(T jump) where T : class, IContextJump
  {
    this.CursorModification = jump;
  }

  public void SetVariable(string name, string val)
  {
    this.ParentContext.SetVariable(name, val);
    this.SetDirty(name);
  }

  public void DeclareVariable(string name)
  {
    this.ParentContext.DeclareVariable(name);
    this.SetDirty(name);
  }

  public void DeclareAndSetConst(string name, string val)
  {
    this.ParentContext.DeclareAndSetConst(name, val);
    this.SetDirty(name);
  }

  public ContextValue? GetVariable(string name)
  {
    return this.ParentContext.GetVariable(name);
  }

  private void SetDirty(string name)
  {
    if (!this.DirtyVariables.Contains(name))
    {
      this.DirtyVariables.Add(name);
    }
  }
}
