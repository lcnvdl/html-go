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
      return new CallArgumentModel() { ArgumentType = "string", Content = content, Html = elementHtmlDefinition.Html };
    }
    else if (elementHtmlDefinition.HasClass("number", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "number", Content = content, Html = elementHtmlDefinition.Html };
    }
    else if (elementHtmlDefinition.HasClass("solve", StringComparison.InvariantCultureIgnoreCase))
    {
      var argument = new CallArgumentModel() { ArgumentType = "solve", Content = content, Html = elementHtmlDefinition.Html };

      foreach (var child in elementHtmlDefinition.Children)
      {
        var childArgument = NewInstance(child, child.InnerText.Trim());

        if (childArgument != null)
        {
          argument.NestedArguments ??= new List<CallArgumentModel>();
          argument.NestedArguments.Add(childArgument);
        }
      }

      return argument;
    }
    else if (elementHtmlDefinition.HasClass("call", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "call", Content = content, Html = elementHtmlDefinition.Html };
    }
    else if (elementHtmlDefinition.HasClass("callReference", StringComparison.InvariantCultureIgnoreCase))
    {
      return new CallArgumentModel() { ArgumentType = "callReference", Content = content, Html = elementHtmlDefinition.Html };
    }
    else if (elementHtmlDefinition.TagName.Equals("a", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(elementHtmlDefinition.GetAttribute("href")))
    {
      string? hrefContent = elementHtmlDefinition.GetAttribute("href");
      string alias = elementHtmlDefinition.InnerText.Trim();
      return new CallArgumentModel() { ArgumentType = "string", Content = hrefContent, Alias = string.IsNullOrEmpty(alias) ? null : alias, Html = elementHtmlDefinition.Html };
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