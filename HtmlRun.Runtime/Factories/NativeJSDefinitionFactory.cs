using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Factories;

static class NativeJSDefinitionFactory
{
  internal static INativeJSDefinition NewInstance(INativeJSBaseInstruction instruction)
  {
    if (instruction is INativeJSInstruction jsInstruction)
    {
      return new NativeJSNetDefinition(jsInstruction.ToJSAction());
    }
    else if (instruction is INativeJSEvalInstruction jsEvalInstruction)
    {
      return new NativeJSEvalDefinition(jsEvalInstruction.ToEvalFunction());
    }
    else
    {
      throw new InvalidOperationException($"Invalid instruction type: {instruction.GetType().Name}.");
    }
  }
}
