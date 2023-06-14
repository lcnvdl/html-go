using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

public class Context : BaseContext, IRuntimeContext
{
  private Dictionary<string, ContextValue> variables = new Dictionary<string, ContextValue>();

  private Context? parent = null;

  private Stack<Context> ctxStack;

  public List<string> Usings { get; private set; } = new();

  public ContextValue[] AllVariables
  {
    get => this.variables.Values.ToArray<ContextValue>();

    set
    {
      this.variables.Clear();

      foreach (var variable in value)
      {
        this.variables[variable.Name] = variable;
      }
    }
  }

  public Context(Context? parent, Stack<Context> ctxStack)
  {
    this.parent = parent;
    this.ctxStack = ctxStack;

    if (parent != null)
    {
      this.AllVariables = parent.AllVariables;
    }
  }

  public ICurrentInstructionContext Fork(IHtmlRuntimeForContext runtimeForContext, string callName, IEnumerable<ParsedArgument> args)
  {
    return new CurrentInstructionContext(runtimeForContext, this, this.ctxStack, callName, args);
  }

  public void DeclareAndSetConst(string name, string val)
  {
    if (this.variables.ContainsKey(name))
    {
      throw new InvalidOperationException($"Variable or constant {name} was already declared.");
    }

    this.variables[name] = new ContextValue(name, val, true);
  }

  public void DeclareVariable(string name)
  {
    if (this.variables.ContainsKey(name))
    {
      throw new InvalidOperationException($"Variable or constant {name} was already declared.");
    }

    this.variables[name] = new ContextValue(name);
  }

  public void SetVariable(string name, string? val)
  {
    if (!this.variables.ContainsKey(name))
    {
      throw new InvalidOperationException($"Variable {name} was not declared.");
    }

    this.variables[name].Value = val;
  }

  public void AddVariable(ContextValue value)
  {
    this.variables[value.Name] = value;
  }

  public ContextValue? GetVariable(string name)
  {
    if (this.variables.TryGetValue(name, out var metadata))
    {
      return metadata;
    }

    return null;
  }

  public void DeleteVariable(string name)
  {
    if (this.variables.ContainsKey(name))
    {
      this.variables.Remove(name);

      if (this.parent != null)
      {
        this.parent.DeleteVariable(name);
      }
    }
  }

  public void AddUsing(string namesp)
  {
    if (!this.Usings.Contains(namesp))
    {
      this.Usings.Add(namesp);
    }
  }
}
