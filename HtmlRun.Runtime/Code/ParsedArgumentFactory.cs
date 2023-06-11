using HtmlRun.Common.Models;
using HtmlRun.Runtime.Utils;

namespace HtmlRun.Runtime.Code;

public static class ParsedArgumentFactory
{
  public static ParsedArgument CreateFromCallArgumentModel(CallArgumentModel argModel, JavascriptParserWithContext jsParser)
  {
    ParsedArgument result;

    if (argModel.IsCall)
    {
      if (string.IsNullOrEmpty(argModel.Content))
      {
        result = ParsedArgument.Null;
      }
      else
      {
        result = new ParsedArgument(jsParser.ExecuteCode(argModel.Content!)?.ToString(), ParsedArgumentType.Native);
      }
    }
    else if (argModel.IsSolve)
    {
      if (string.IsNullOrEmpty(argModel.Content))
      {
        result = ParsedArgument.Null;
      }
      else
      {
        result = new ParsedArgument(JavascriptParser.SimpleSolve(argModel.Content!)?.ToString(), ParsedArgumentType.Native);
      }
    }
    else if (argModel.IsPrimitive)
    {
      result = new ParsedArgument(argModel.Content, argModel.IsNumber ? ParsedArgumentType.Number : ParsedArgumentType.String);
    }
    else if (argModel.IsCallReference)
    {
      if (!string.IsNullOrEmpty(argModel.Content))
      {
        string code = argModel.Content!;
        string keyId = CryptoUtils.HashWithSHA256(code);
        string key = $"{Constants.SpecialNames.CallReferenceVariableName}_{keyId}";

        result = new ParsedArgument(key, ParsedArgumentType.Reference);
      }
      else
      {
        result = ParsedArgument.Null;
      }
    }
    else if (argModel.IsBranch)
    {
      result = ParsedArgument.Null;
    }
    else
    {
      throw new InvalidDataException($"Unknown argument type {argModel.ArgumentType}.");
    }

    return result;
  }
}