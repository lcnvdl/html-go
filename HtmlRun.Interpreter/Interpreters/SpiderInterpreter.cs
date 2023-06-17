using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Factories;
using HtmlRun.Interpreter.HtmlParser;
using HtmlRun.Interpreter.Interpreters.Parsers;

namespace HtmlRun.Interpreter.Interpreters;

public class SpiderInterpreter : IInterpreter
{
  private int nextCallModelIdx = 0;

  public async Task<AppModel> ParseString(string content)
  {
    IParser parser = new AngleSharpParser();
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

    //  * Classes

    //  TODO  Read classes from <table>

    //  * Entities

    program.Entities = parser.BodyQuerySelectorAll("table.entity").Select(EntityParser.ParseTable).Where(m => m != null).Cast<EntityModel>().ToList();

    //  Result

    return program;
  }

  private CallModel? ParseCall(IHtmlElementAbstraction li)
  {
    if (
      li.HasClass("comment", StringComparison.InvariantCultureIgnoreCase) ||
      li.HasClass("ignore", StringComparison.InvariantCultureIgnoreCase))
    {
      return null;
    }

    var special = li.FindChildByTag("b");
    var call = special ?? li.FindChildByTag("u");

    if (call == null)
    {
      throw new NullReferenceException($"Call not found in line {li.InnerHtml}.");
    }

    List<IHtmlElementAbstraction> callArguments = li.Children.Skip(1).ToList();

    var model = new CallModel();
    model.Index = Interlocked.Increment(ref this.nextCallModelIdx);
    model.CustomId = li.GetData("id") ?? call.GetData("id");
    model.FunctionName = call.InnerHtml.Trim();
    model.IsSpecial = special != null;
    model.Arguments.AddRange(this.ParseCallArguments(callArguments, model.IsSpecial, model.FunctionName));

    return model;
  }

  private IEnumerable<CallArgumentModel> ParseCallArguments(List<IHtmlElementAbstraction> callArguments, bool isSpecialModel, string modelFunctionName)
  {
    int argumentIndex = 0;
    foreach (var argHtmlDefinition in callArguments)
    {
      //  Branched instructions

      if (isSpecialModel && argHtmlDefinition.TagName.Equals("ul", StringComparison.InvariantCultureIgnoreCase))
      {
        CallArgumentModel branch = CallArgumentFactory.NewInstanceFromBranch(
          argHtmlDefinition.GetData("condition"),
          argHtmlDefinition.Children.Select(this.ParseCall).Where(m => m != null).Cast<CallModel>().ToList()
        );

        yield return branch;
      }
      else
      {
        //  Linear instructions

        string content = HtmlDecode(argHtmlDefinition.InnerHtml).Trim();

        CallArgumentModel? argModel = CallArgumentFactory.NewInstance(argHtmlDefinition, content);

        if (argModel == null)
        {
          throw new InvalidDataException($"Unknown type of argument #{argumentIndex} on call {modelFunctionName}.");
        }

        yield return argModel;
      }

      ++argumentIndex;
    }
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