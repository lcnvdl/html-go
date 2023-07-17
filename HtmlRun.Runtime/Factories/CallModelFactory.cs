using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;

namespace HtmlRun.Runtime.Factories;

static class CallModelFactory
{
  internal static CallModel NewCall(string key, params CallArgumentModel[] args)
  {
    var model = new CallModel();
    model.FunctionName = key;
    model.Arguments.AddRange(args);
    return model;
  }

  internal static CallModel Goto(string labelName)
  {
    return NewCall(Constants.BasicInstructionsSet.Goto, CallArgumentModel.FromString(labelName));
  }


  internal static CallModel Return()
  {
    return NewCall(Constants.BasicInstructionsSet.Return);
  }

  internal static CallModel TestAndGoto(CallArgumentModel ifCondition, string expectedValue, string labelName)
  {
    var testAndGoto = NewCall(Constants.BasicInstructionsSet.TestAndGoto, CallArgumentModel.FromString(expectedValue), CallArgumentModel.FromString(labelName));
    testAndGoto.Arguments.Insert(0, ifCondition);
    return testAndGoto;
  }

  internal static CallModel Label(string labelName)
  {
    var label = NewCall(Constants.BasicInstructionsSet.Label, CallArgumentModel.FromString(labelName));
    label.CustomId = labelName;
    return label;
  }

  internal static CallModel PushContext()
  {
    return NewCall(Constants.BasicInstructionsSet.ContextPush);
  }

  internal static CallModel PullContext()
  {
    return NewCall(Constants.BasicInstructionsSet.ContextPull);
  }
}