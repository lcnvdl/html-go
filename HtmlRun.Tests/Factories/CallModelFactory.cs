using HtmlRun.Common.Models;
using HtmlRun.Runtime.Code;

public static class CallModelFactory
{
  public static CallModel NewCall(string key, params CallArgumentModel[] args)
  {
    var model = new CallModel();
    model.FunctionName = key;
    model.Arguments.AddRange(args);
    return model;
  }

  public static CallModel AsCall(this string key, params CallArgumentModel[] args)
  {
    return NewCall(key, args);
  }
}