using HtmlRun.Common.Models;

namespace HtmlRun.Interpreter.Factories;

static class CallArgumentFactory
{
  internal static CallArgumentModel? GetInstance(IHtmlElementAbstraction elementHtmlDefinition, string content)
  {
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
    else
    {
      return null;
    }
  }
}