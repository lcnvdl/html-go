using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Code;

static class JavascriptParserWithContextFactory
{
  internal static JavascriptParserWithContext CreateNewJavascriptParserAndAssignInstructions(Dictionary<string, INativeJSDefinition> jsInstructions)
  {
    var jsParserWithContext = new JavascriptParserWithContext();

    AssignInstructions(jsParserWithContext, jsInstructions);

    return jsParserWithContext;
  }

  internal static void AssignInstructions(JavascriptParserWithContext jsParserWithContext, Dictionary<string, INativeJSDefinition> jsInstructions)
  {
    foreach (var kv in jsInstructions)
    {
      if (kv.Value is NativeJSNetDefinition netDefinition)
      {
        AssignNetDefinitionInstruction(jsParserWithContext, kv.Key, netDefinition);
      }
      else if (kv.Value is NativeJSEvalDefinition evalDefinition)
      {
        AssignEvalDefinitionInstruction(jsParserWithContext, kv.Key, evalDefinition);
      }
      else
      {
        throw new InvalidOperationException($"Invalid instruction type: {kv.Value.GetType().Name}.");
      }
    }
  }

  private static void AssignEvalDefinitionInstruction(JavascriptParserWithContext jsParserWithContext, string name, NativeJSEvalDefinition definition)
  {
    string evalValue = "{ " + definition.Function + " }";

    if (definition.Arguments < 0)
    {
      evalValue = "function(args)" + evalValue;
    }
    else if (definition.Arguments == 0)
    {
      evalValue = "function()" + evalValue;
    }
    else
    {
      evalValue = "function(" + string.Join(",", Enumerable.Range(0, definition.Arguments).Select(i => $"arg{i}")) + ")" + evalValue;
    }

    SanitizeAndRegister(
      jsParserWithContext,
      name,
      sanitizedKey => { jsParserWithContext.RegisterFunction("window." + sanitizedKey + "=" + evalValue); });
  }

  private static void AssignNetDefinitionInstruction(JavascriptParserWithContext jsParserWithContext, string name, NativeJSNetDefinition definition)
  {
    SanitizeAndRegister(
      jsParserWithContext,
      name,
      sanitizedKey => { jsParserWithContext.RegisterInstruction(sanitizedKey, definition.Delegate); });
  }

  private static void SanitizeAndRegister(JavascriptParserWithContext jsParserWithContext, string name, Action<string> register)
  {
    if (name.Contains('.'))
    {
      string sanitizedKey = "call__" + name.Replace(".", "__");
      register(sanitizedKey);

      var split = name.Split('.');

      for (int i = 0; i < split.Length; i++)
      {
        string curr = split[i];

        for (int j = i - 1; j >= 0; j--)
        {
          curr = $"{split[j]}.{curr}";
        }

        jsParserWithContext.ExecuteCode($"window.{curr}=window.{curr}||{{}};");
      }

      jsParserWithContext.ExecuteCode($"window.{name}={sanitizedKey}");
    }
    else
    {
      register(name);
    }
  }
}