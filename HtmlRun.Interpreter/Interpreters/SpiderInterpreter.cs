using System.Linq;
using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Factories;
using HtmlRun.Interpreter.HtmlParser;

namespace HtmlRun.Interpreter.Interpreters;

public class SpiderInterpreter : IInterpreter
{
  private int nextCallModelIdx = 0;

  public async Task<AppModel> ParseString(string content)
  {
    var parser = new AngleSharpParser();
    await parser.Load(content);

    //  App

    var program = new AppModel();

    program.Title = parser.HeadTitle;

    //  * Calls

    var possibleCalls = parser.BodyQuerySelectorAll("body > ul.calls > li");
    if (!possibleCalls.Any())
    {
      possibleCalls = parser.BodyQuerySelectorAll("body > ul > li");
    }

    program.Instructions = possibleCalls.Select(this.ParseCall).Where(m => m != null).Cast<CallModel>().ToList();

    //  * Functions

    program.Functions = parser.BodyQuerySelectorAll(".code").Select(this.ParseCode).Where(m => m != null).Cast<FunctionModel>().ToList();

    return program;
  }

  private CallModel? ParseCall(IHtmlElementAbstraction li)
  {
    if (li.HasClass("comment", StringComparison.InvariantCultureIgnoreCase) ||
    li.HasClass("ignore", StringComparison.InvariantCultureIgnoreCase))
    {
      return null;
    }

    var special = li.FindChildByTag("b");

    var call = special ?? li.FindChildByTag("u");
    if (call == null)
    {
      throw new NullReferenceException("Call not found.");
    }

    var model = new CallModel();

    model.Index = this.nextCallModelIdx++;
    model.FunctionName = call.InnerHtml.Trim();
    model.IsSpecial = special != null;

    var args = li.Children.Skip(1).ToList();

    int argumentIndex = 0;
    foreach (var argHtmlDefinition in args)
    {
      if (model.IsSpecial && argHtmlDefinition.TagName.Equals("ul", StringComparison.InvariantCultureIgnoreCase))
      {
        var branch = new CallArgumentModel();
        branch.ArgumentType = "branch";
        branch.BranchCondition = argHtmlDefinition.GetData("condition");
        branch.BranchInstructions = argHtmlDefinition.Children.Select(this.ParseCall).Where(m => m != null).Cast<CallModel>().ToList();
        model.Arguments.Add(branch);
      }
      else
      {
        string content = HtmlDecode(argHtmlDefinition.InnerHtml).Trim();

        CallArgumentModel? argModel = CallArgumentFactory.GetInstance(argHtmlDefinition, content);

        if (argModel == null)
        {
          throw new InvalidDataException($"Unknown type of argument #{argumentIndex} on call {model.FunctionName}");
        }

        model.Arguments.Add(argModel);
      }

      ++argumentIndex;
    }

    return model;
  }

  private FunctionModel? ParseCode(IHtmlElementAbstraction e)
  {
    var pre = e.FindChildByTag("pre");
    if (pre == null)
    {
      return null;
    }

    var model = new FunctionModel();
    model.Id = e.ElementId;
    model.Code = HtmlDecode(pre.InnerText);

    var argumentsContainer = e.FindChildByTag("h3");
    if (argumentsContainer != null)
    {
      var argsList = argumentsContainer.Children.ToList();
      model.Arguments.AddRange(argsList.Select(m => m.InnerText.Trim()));
    }

    return model;
  }

  private static string HtmlDecode(string html)
  {
    return System.Net.WebUtility.HtmlDecode(html).Trim();
  }
}