using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Factories;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

public class Context : BaseContext, IRuntimeContext
{
  private readonly Dictionary<string, ContextValue> variables = new();

  private readonly Context? parent;

  private readonly Stack<Context> ctxStack;

  private readonly Stack<GroupArguments> argsStack;

  private readonly IHeap heap;

  internal Stack<Context> CtxStack => this.ctxStack;

  public List<string> Usings { get; private set; } = new();

  public Context? Parent => this.parent;

  public IHeap Heap => this.heap;

  public ContextValue[] AllHierarchyVariables
  {
    get
    {
      var result = new List<ContextValue>();

      if (this.parent != null)
      {
        result.AddRange(this.parent.AllHierarchyVariables);
      }

      result.AddRange(this.AllVariables);

      return result.ToArray();
    }
  }

  public ContextValue[] AllVariables
  {
    get => this.variables.Values.ToArray();

    set
    {
      this.variables.Clear();

      foreach (var variable in value)
      {
        this.variables[variable.Name] = variable;
      }
    }
  }

  public Context(Context? parent, Stack<Context> ctxStack, Stack<GroupArguments> argsStack, IHeap? heap = null)
  {
    this.parent = parent;
    this.ctxStack = ctxStack;
    this.argsStack = argsStack;
    this.heap = heap ?? new Heap();

    if (parent != null)
    {
      this.AllVariables = parent.AllVariables;
    }
  }

  public Context Fork()
  {
    return new Context(this, this.ctxStack, this.argsStack);
  }

  public void ClearArguments()
  {
    this.argsStack.Clear();
  }

  public void InitialPushArgumentsAndValues(GroupArguments arguments)
  {
    if(this.argsStack.Count > 0)
    {
      throw new InvalidOperationException("Cannot push arguments and values when there are already arguments and values in the stack.");
    }

    this.argsStack.Push(arguments);
  }

  public ICurrentInstructionContext Fork(IHtmlRuntimeForContext runtimeForContext, string callName, IEnumerable<ParsedArgument> args)
  {
    return new CurrentInstructionContext(runtimeForContext, this, this.ctxStack, this.argsStack, callName, args);
  }

  public void DeclareAndSetConst(string name, string val)
  {
    //  TODO  Set up this validation
    const bool skipHeapValidation = true;

    if (skipHeapValidation ? this.VariableExists(name) : this.VariableOrHeapExists(name))
    {
      throw new InvalidOperationException($"Variable or constant {name} was already declared.");
    }

    this.variables[name] = new ContextValue(name, val, true);
  }

  public void DeclareVariable(string name)
  {
    //  TODO  Set up this validation
    const bool skipHeapValidation = true;

    if (skipHeapValidation ? this.VariableExists(name) : this.VariableOrHeapExists(name))
    {
      throw new InvalidOperationException($"Variable or constant {name} was already declared.");
    }

    this.variables[name] = new ContextValue(name);
  }

  public void ExportVariable(string name)
  {
    var variable = this.GetVariable(name) ?? throw new InvalidOperationException($"Variable {name} was not declared.");
    variable.IsExported = true;
  }

  public void SetValueVariable(string name, string? val)
  {
    var variable = this.GetVariable(name) ?? throw new InvalidOperationException($"Variable {name} was not declared.");
    variable.Value = val;
  }

  public bool IsDeclared(string name, bool ignoreInferred = false)
  {
    return ignoreInferred ? this.variables.ContainsKey(name) : this.VariableExists(name);
  }

  public ContextValue? GetVariable(string name)
  {
    if (this.variables.TryGetValue(name, out var metadata))
    {
      return metadata;
    }

    return this.GetInferredVariable(name);
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

  public int AllocInHeap(object val)
  {
    if (val is EntityModel entity)
    {
      var instanceValues = new Dictionary<string, object?>();

      foreach (var attribute in entity.Attributes)
      {
        instanceValues[attribute.Name] = attribute.DefaultValue ?? (attribute.IsNull ? null : "");
      }

      return this.Heap.Alloc(val, instanceValues);
    }
    else
    {
      return this.Heap.Alloc(val);
    }
  }

  public void Release()
  {
    GarbageCollector.CollectFromReleasedContext(this);
  }

  public void AddVariable(ContextValue value)
  {
    this.variables[value.Name] = value;
  }

  public EntityModel? PointerToEntity(int ptr)
  {
    return this.Heap.ItemsRef[ptr].Data as EntityModel;
  }

  private bool VariableOrHeapExists(string name)
  {
    if (name.Contains('.'))
    {
      return true;
    }

    return VariableExists(name);
  }

  private bool VariableExists(string name)
  {
    return this.variables.ContainsKey(name) || this.GetInferredVariable(name) != null;
  }

  private ContextValue? GetInferredVariable(string name)
  {
    if (!name.Contains('.'))
    {
      return null;
    }

    string possibleInstanceName = name.Remove(name.IndexOf('.'));

    if (this.variables.ContainsKey(possibleInstanceName))
    {
      var possibleReferenceValue = this.variables[possibleInstanceName].Value;

      if (possibleReferenceValue != null && int.TryParse(possibleReferenceValue, out int refIndex))
      {
        if (refIndex < this.heap.ItemsRef.Count)
        {
          var heapItem = this.heap.ItemsRef[refIndex];

          var inferredValues = InferredContextValueFactory.InferValuesFromHeapItem(heapItem);

          string varName = name.Substring(name.LastIndexOf('.') + 1);

          return inferredValues.FirstOrDefault(m => m.Name == varName);
        }
      }
    }

    return null;
  }

  public void AllocAndSetPointerVariable(string name, object val)
  {
    this.SetValueVariable(name, this.AllocInHeap(val).ToString());
  }
}
