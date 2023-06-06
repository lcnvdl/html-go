using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime;

public class Context : IRuntimeContext
{
  private Context? parent = null;

  private Stack<Context> ctxStack;

  private string? callName;

  private List<string?>? args;

  public string? CursorModification { get; set; }

  public Context(Context? parent, Stack<Context> ctxStack)
  {
    this.parent = parent;
    this.ctxStack = ctxStack;
  }

  public Context(Context? parent, Stack<Context> ctxStack, string callName, IEnumerable<string?> args)
  {
    this.parent = parent;
    this.ctxStack = ctxStack;
    this.callName = callName;
    this.args = args.ToList();
  }

  public T? GetArgument<T>(int idx = 0, string? errorMessage = null)
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

  public string?[] GetArguments()
  {
    return this.args?.ToArray() ?? new string?[] { };
  }

  public Context Fork(string callName, IEnumerable<string?> args)
  {
    return new Context(this, this.ctxStack, callName, args);
  }

  public void JumpToBranch(string condition)
  {
    this.CursorModification = condition;
  }
}
