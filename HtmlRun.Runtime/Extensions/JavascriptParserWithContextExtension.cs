using HtmlRun.Runtime.Code;

namespace HtmlRun.Runtime;

public static class JavascriptParserWithContextExtension
{
  public static void SafeSet(this JavascriptParserWithContext jsParserWithContext, string variableKey, string? value)
  {
    if (string.IsNullOrEmpty(variableKey))
    {
      throw new InvalidOperationException("Variable name is empty.");
    }

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

    if (value == null)
    {
      jsParserWithContext.ExecuteCode($"window.{variableKey}=null");
    }
    else
    {
      jsParserWithContext.ExecuteCode($"window.{variableKey}='{value}'");
    }
  }
}