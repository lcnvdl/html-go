using HtmlRun.Common.Models;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Interfaces;

public interface IBaseContext
{
  ContextValue[] AllVariables { get; }

  ContextValue? GetVariable(string name);
  
  void DeleteVariable(string name);

  bool ExistsVariable(string name) => this.GetVariable(name) != null;

  void SetValueVariable(string name, string? val);
  
  int AllocInHeap(object val);

  void AllocAndSetPointerVariable(string name, object val);
    
  EntityModel? PointerToEntity(int ptr);

  void DeclareVariable(string name);
  
  void DeclareAndSetConst(string name, string val);

}
