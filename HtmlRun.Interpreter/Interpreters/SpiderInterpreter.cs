using System.Linq;
using HtmlRun.Common.Models;
using HtmlRun.Interpreter.HtmlParser;

namespace HtmlRun.Interpreter.Interpreters;

public class SpiderInterpreter : IInterpreter
{
  public async Task<AppModel> ParseString(string content)
  {
    var parser = new AngleSharpParser();
    await parser.Load(content);

    //  App

    var program = new AppModel();

    program.Title = parser.HeadTitle;

    //  * Calls

    var possibleCalls = parser.BodyQuerySelectorAll("ul.calls > li");
    if (!possibleCalls.Any())
    {
      possibleCalls = parser.BodyQuerySelectorAll("ul > li");
    }

    program.Instructions = possibleCalls.Select(this.ParseCall).Where(m => m != null).Cast<CallModel>().ToList();

    //  * Functions

    program.Functions = parser.BodyQuerySelectorAll(".code").Select(this.ParseCode).Where(m => m != null).Cast<FunctionModel>().ToList();

    return program;
  }

  private CallModel? ParseCall(IHtmlElementAbstraction e)
  {
    if (e.HasClass("comment", StringComparison.InvariantCultureIgnoreCase))
    {
      return null;
    }

    var call = e.Children.FirstOrDefault(m => m.TagName.Equals("u", StringComparison.InvariantCultureIgnoreCase));
    if (call == null)
    {
      throw new NullReferenceException("Call not found.");
    }

    var model = new CallModel();

    model.FunctionName = call.InnerHtml.Trim();

    var args = e.Children.Skip(1).ToList();

    int idx = 0;
    foreach (var arg in args)
    {
      string content = arg.InnerHtml.Trim();

      if (arg.HasClass("string", StringComparison.InvariantCultureIgnoreCase))
      {
        model.Arguments.Add(new CallArgumentModel() { ArgumentType = "string", Content = content });
      }
      else if (arg.HasClass("solve", StringComparison.InvariantCultureIgnoreCase))
      {
        model.Arguments.Add(new CallArgumentModel() { ArgumentType = "solve", Content = content });
      }
      else if (arg.HasClass("call", StringComparison.InvariantCultureIgnoreCase))
      {
        model.Arguments.Add(new CallArgumentModel() { ArgumentType = "call", Content = content });
      }
      else
      {
        throw new InvalidDataException($"Unknown type of argument #{idx} on call {model.FunctionName}");
      }

      ++idx;
    }

    return model;
  }

  private FunctionModel? ParseCode(IHtmlElementAbstraction e)
  {
    var pre = e.Children.FirstOrDefault(m => m.TagName.Equals("pre", StringComparison.InvariantCultureIgnoreCase));
    if (pre == null)
    {
      return null;
    }

    var model = new FunctionModel();
    model.Id = e.ElementId;
    model.Code = System.Net.WebUtility.HtmlDecode(pre.InnerText).Trim();

    var argumentsContainer = e.Children.FirstOrDefault(m => m.TagName.Equals("h3", StringComparison.InvariantCultureIgnoreCase));
    if (argumentsContainer != null)
    {
      var argsList = argumentsContainer.Children.ToList();
      model.Arguments.AddRange(argsList.Select(m => m.InnerText.Trim()));
    }

    return model;
  }
}