using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Interfaces;

public interface IBaseContext
{
  ContextValue[] AllVariables { get; }

  ContextValue? GetVariable(string name);

  void SetVariable(string name, string val);
  
  void DeclareVariable(string name);
  
  void DeclareAndSetConst(string name, string val);
}
