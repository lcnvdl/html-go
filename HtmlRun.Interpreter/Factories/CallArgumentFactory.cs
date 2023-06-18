using System.Text.RegularExpressions;
using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;

namespace HtmlRun.Interpreter.Factories;

static class CallArgumentFactory
{
  internal static CallArgumentModel? NewInstance(IHtmlElementAbstraction elementHtmlDefinition, string content)
  {
    if (elementHtmlDefinition.HasClass("preprocess", StringComparison.InvariantCultureIgnoreCase))
    {
      var matches = Regex.Matches(content, @"([\$](\w+))\b");
      foreach (Match match in matches)
      {
        string varName = match.Value.Substring(1);
        var envVar = Environment.GetEnvironmentVariable(varName);
        if (envVar != null)
        {
          content = content.Replace(match.Value, envVar);
        }
      }
    }

    if (elementHtmlDefinition.HasClass("string", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "string", Content = content };
    }
    else if (elementHtmlDefinition.HasClass("number", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "number", Content = content };
    }
    else if (elementHtmlDefinition.HasClass("solve", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "solve", Content = content };
    }
    else if (elementHtmlDefinition.HasClass("call", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "call", Content = content };
    }
    else if (elementHtmlDefinition.HasClass("callReference", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "callReference", Content = content };
    }
    else
    {
      return null;
    }
  }

  internal static CallArgumentModel NewInstanceFromBranch(string? condition, List<CallModel> subInstructions)
  {
    var branch = new CallArgumentModel();
    branch.ArgumentType = "branch";
    branch.BranchCondition = condition;
    branch.BranchInstructions = subInstructions;
    return branch;
  }
}