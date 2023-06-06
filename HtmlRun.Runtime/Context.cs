using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

public class Context : IRuntimeContext
{
  private Context? parent = null;

  private Stack<Context> ctxStack;

  private string? callName;

  private List<string?>? args;

  public IContextJump? CursorModification { get; set; }

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

  public void Jump<T>(T jump) where T : class, IContextJump
  {
    this.CursorModification = jump;
  }
}
