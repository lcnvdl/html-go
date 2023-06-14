namespace HtmlRun.Runtime.Code;

static class JavascriptParserWithContextFactory
{
  internal static JavascriptParserWithContext CreateNewJavascriptParserAndAssignInstructions(Dictionary<string, Delegate> jsInstructions)
  {
    var jsParserWithContext = new JavascriptParserWithContext();

    foreach (var kv in jsInstructions)
    {
      if (kv.Key.Contains("."))
      {
        string sanitizedKey = "call__" + kv.Key.Replace(".", "__");
        jsParserWithContext.RegisterInstruction(sanitizedKey, kv.Value);

        var split = kv.Key.Split('.');

        for (int i = 0; i < split.Length; i++)
        {
          string curr = split[i];

          for (int j = i - 1; j >= 0; j--)
          {
            curr = $"{split[j]}.{curr}";
          }

          jsParserWithContext.ExecuteCode($"window.{curr}=window.{curr}||{{}};");
        }

        jsParserWithContext.ExecuteCode($"window.{kv.Key}={sanitizedKey}");
      }
      else
      {
        jsParserWithContext.RegisterInstruction(kv.Key, kv.Value);
      }
    }

    return jsParserWithContext;
  }
}