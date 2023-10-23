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
        if (variableKey.Contains('.'))
        {
          var accumulatedParts = new List<string>();

          var parts = variableKey.Split('.');
          jsParserWithContext.ExecuteCode($"if(typeof window.{parts.First()} !== 'object') {{ delete window.{parts.First()}; }}");

          foreach (string part in parts)
          {
            accumulatedParts.Add(part);
            string accumulatedKey = string.Join(".", accumulatedParts);

            jsParserWithContext.ExecuteCode($"window.{accumulatedKey}=window.{accumulatedKey}||{{}}");
          }
        }

        if (metaVariable.IsPointer)
        {
          var heapEntry = finalCtx.ParentContext.Heap.Read(int.Parse(metaVariable.Value!));
          var rawValue = heapEntry.Data;

          if (rawValue != null && rawValue.GetType()!.FullName!.Contains("Jurassic"))
          {
            jsParserWithContext.Engine.SetGlobalValue("tmpPointer", rawValue);

            jsParserWithContext.ExecuteCode($"window.{variableKey}=tmpPointer");

            jsParserWithContext.ExecuteCode("delete window.tmpPointer");
          }
          else
          {
            //  TODO  Set values from heap
          }
        }
        else
        {
          jsParserWithContext.ExecuteCode($"window.{variableKey}='{metaVariable.Value}'");
        }
      }
    }

    finalCtx.DirtyVariables.Clear();
  }
}