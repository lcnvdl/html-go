using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Factories;
using HtmlRun.Interpreter.HtmlParser;
using HtmlRun.Interpreter.Interpreters.Parsers;

namespace HtmlRun.Interpreter.Interpreters;

public class SpiderInterpreter : IInterpreter
{
  private int nextCallModelIdx = 0;

  public async Task<AppModel> ParseString(string initialReference, Func<string, string> readContent)
  {
    IParser parser = new AngleSharpParser();
    await parser.Load(readContent(initialReference));

    string directory = Path.GetDirectoryName(initialReference) ?? string.Empty;

    //  App

    var program = new AppModel();
    program.Id = parser.GetMetaContentWithDefaultValue("htmlgo:application-id", program.Id ?? Guid.NewGuid().ToString());
    program.Version = parser.GetMetaContentWithDefaultValue("htmlgo:application-version", "1.0.0");
    program.Type = GetAppTypeFromString(parser.GetMetaContentWithDefaultValue("htmlgo:application-type"));
    program.Title = parser.HeadTitle;

    //  * Calls

    var possibleGroups = parser.BodyQuerySelectorAll("body > ul");

    program.InstructionGroups.AddRange(this.ParseInstructionGroupsAndCalls(possibleGroups));

    //  * Functions

    program.Functions = parser.BodyQuerySelectorAll(".code").Select(this.ParseCode).Where(m => m != null).Cast<FunctionModel>().ToList();

    //  * Classes

    //  TODO  Read classes from <table>

    //  * Entities

    program.Entities = parser.BodyQuerySelectorAll("table.entity").Select(EntityParser.ParseTable).Where(m => m != null).Cast<EntityModel>().ToList();

    //  * Imports

    foreach (var group in program.InstructionGroups)
    {
      var importInstructions = group.Instructions.FindAll(m => m.IsSpecial && m.FunctionName == "Import");

      foreach (var import in importInstructions)
      {
        if (string.IsNullOrEmpty(import.Arguments[0].Content))
        {
          throw new NullReferenceException($"Import path not found in line {import.CustomId}.");
        }

        string path = import.Arguments[0].Content!;

        if (!string.IsNullOrEmpty(directory))
        {
          path = Path.Combine(directory, path);
        }

        AppModel library = await this.ParseString(path, readContent);
        string? alias = (import.Arguments.Count > 1 ? import.Arguments[1].Content : null) ?? import.Arguments[0].Alias;

        var importedData = new ImportedLibraryModel(program, path, library, alias);

        program.Imports.Add(importedData);
      }
    }

    //  Result

    return program;
  }

  private List<InstructionsGroup> ParseInstructionGroupsAndCalls(IEnumerable<IHtmlElementAbstraction> possibleGroups)
  {
    var groups = new List<InstructionsGroup>();
    var allPossibleGroups = possibleGroups.ToList();

    InstructionsGroup? mainGroup = null;

    foreach (var groupElement in allPossibleGroups)
    {
      if (groupElement.HasClass("ignore", StringComparison.InvariantCultureIgnoreCase) ||
        groupElement.HasClass("comment", StringComparison.InvariantCultureIgnoreCase))
      {
        continue;
      }

      string label = groupElement.GetData("label") ?? InstructionsGroup.MainLabel;

      bool isMain = label.Equals(InstructionsGroup.MainLabel, StringComparison.InvariantCultureIgnoreCase);

      var possibleCalls = groupElement.Children;

      if (isMain)
      {
        if (mainGroup == null)
        {
          mainGroup = InstructionsGroup.Main;
          groups.Add(mainGroup);
        }

        mainGroup.Instructions.AddRange(this.ParseInstructionOfGroup(possibleCalls));
      }
      else
      {
        var newGroup = new InstructionsGroup(label);
        newGroup.Instructions.AddRange(this.ParseInstructionOfGroup(possibleCalls));

        string? argumentsData = groupElement.GetData("arguments");

        if (!string.IsNullOrEmpty(argumentsData))
        {
          var arguments = argumentsData!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
          newGroup.Arguments.AddRange(arguments.Select(name => new InstructionGroupArgumentModel(name)));
        }

        groups.Add(newGroup);
      }
    }

    return groups;
  }

  private List<CallModel> ParseInstructionOfGroup(IEnumerable<IHtmlElementAbstraction> possibleCalls)
  {
    return possibleCalls.Select(this.ParseCall).Where(m => m != null).Cast<CallModel>().ToList();
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
    model.CallType = special != null ? CallType.Special : CallType.Instruction;
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

  private static AppType GetAppTypeFromString(string? appType)
  {
    if (string.IsNullOrEmpty(appType))
    {
      return AppType.Unknown;
    }

    switch (appType.ToLowerInvariant())
    {
      case "console":
        return AppType.Console;
      case "webserver":
        return AppType.WebServer;
      case "library":
        return AppType.Library;
      default:
        throw new Exception($"App type {appType} is wrong.");
    }
  }
}