using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.RuntimeContext;

public class CurrentInstructionContext : BaseContext, ICurrentInstructionContext, IUnsafeCurrentInstructionContext
{
  private readonly Stack<Context> ctxStack;

  private readonly string? callName;

  private readonly Stack<GroupArguments> argsStack;

  private readonly List<ParsedArgument>? args;

  public IRuntimeContext ParentContext { get; private set; }

  public IContextJump? CursorModification { get; set; }

  public ContextValue[] AllVariables => this.ParentContext.AllVariables;

  public List<string> DirtyVariables { get; private set; } = new List<string>();

  public IHtmlRuntimeForContext Runtime { get; private set; }

  public IHtmlRuntimeForUnsafeContext UnsafeRuntime => (IHtmlRuntimeForUnsafeContext)this.Runtime;

  public CurrentInstructionContext(IHtmlRuntimeForContext runtime, IRuntimeContext parent, Stack<Context> ctxStack, Stack<GroupArguments> argsStack, string callName, IEnumerable<ParsedArgument> args)
  {
    this.Runtime = runtime;
    this.ParentContext = parent;
    this.argsStack = argsStack;
    this.ctxStack = ctxStack;
    this.callName = callName;
    this.args = args.ToList();
  }

  public ParsedArgument GetArgumentAt(int idx)
  {
    if (this.args == null)
    {
      throw new NullReferenceException();
    }

    return this.args[idx];
  }

  public T GetRequiredArgument<T>(int idx = 0, string? errorMessage = null)
  {
    if (this.args == null)
    {
      throw new NullReferenceException();
    }

    if (this.args.Count <= idx || this.args[idx].IsNull)
    {
      throw new ArgumentException(errorMessage ?? $"Argument {idx} is missing on call {this.callName}.");
    }

    var newType = Convert.ChangeType(this.args[idx].Value, typeof(T));

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

    if (this.args.Count <= idx || this.args[idx].IsNull)
    {
      return default;
    }

    var newType = Convert.ChangeType(this.args[idx].Value, typeof(T));

    if (newType == null)
    {
      return default;
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

  public ParsedArgument[] GetArguments()
  {
    return this.args?.ToArray() ?? Array.Empty<ParsedArgument>();
  }

  public void Jump<T>(T jump) where T : class, IContextJump
  {
    this.CursorModification = jump;
  }

  public void SetValueVariable(string name, string? val)
  {
    this.ParentContext.SetValueVariable(name, val);
    this.SetDirty(name);
  }

  public void DeclareVariable(string name)
  {
    this.ParentContext.DeclareVariable(name);
    this.SetDirty(name);
  }

  public void ExportVariable(string name)
  {
    this.ParentContext.ExportVariable(name);
    this.SetDirty(name);
  }

  public void DeleteVariable(string name)
  {
    this.ParentContext.DeleteVariable(name);
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

  public void AddUsing(string namesp)
  {
    this.ParentContext.AddUsing(namesp);
  }

  public void AddVariable(ContextValue value)
  {
    this.ParentContext.AddVariable(value);
  }

  public int AllocInHeap(object value)
  {
    return this.ParentContext.AllocInHeap(value);
  }

  public EntityModel? PointerToEntity(int ptr)
  {
    return this.ParentContext.PointerToEntity(ptr);
  }

  public void AllocAndSetPointerVariable(string name, object val)
  {
    this.ParentContext.AllocAndSetPointerVariable(name, val);

    this.SetAllObjectAsDirty(name, val);
  }

  private void SetAllObjectAsDirty(string variableName, object newInstance)
  {
    if (newInstance is EntityModel entity)
    {
      foreach (var attribute in entity.Attributes)
      {
        this.SetDirty($"{variableName}.{attribute.Name}");
      }
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public void PushArgumentsAndValues(GroupArguments arguments)
  {
    this.argsStack.Push(arguments);
  }

  public GroupArguments? PopArgumentsAndValues()
  {
    return this.argsStack.Count == 0 ? null : this.argsStack.Pop();
  }
}
