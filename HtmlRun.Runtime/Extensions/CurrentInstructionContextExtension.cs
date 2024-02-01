using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime;

public static class CurrentInstructionContextExtension
{
  public static void SaveContextVariableChangesToJsEngine(this ICurrentInstructionContext finalCtx, JavascriptParserWithContext jsParserWithContext)
  {
    foreach (string variableKey in finalCtx.DirtyVariables)
    {
      if (string.IsNullOrEmpty(variableKey))
      {
        throw new InvalidOperationException("Variable name is empty.");
      }

      ContextValue? metaVariable = finalCtx.GetVariable(variableKey);

      if (metaVariable == null)
      {
        jsParserWithContext.ExecuteCode($"delete window.{variableKey}");
      }
      else
      {
        jsParserWithContext.SafeSet(variableKey, metaVariable.Value);
      }
    }
  }
}