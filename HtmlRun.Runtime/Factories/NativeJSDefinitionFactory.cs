using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Factories;

static class NativeJSDefinitionFactory
{
  internal static INativeJSDefinition NewInstance(INativeJSBaseInstruction instruction, Func<JavascriptParserWithContext> jsEngineGetter)
  {
    if (instruction is INativeJSInstruction jsInstruction)
    {
      if (instruction is IInstructionRequiresJsEngine jsEngineRequired)
      {
        jsEngineRequired.SetEngineGenerator(jsEngineGetter);
      }

      return new NativeJSNetDefinition(jsInstruction.ToJSAction());
    }
    else if (instruction is INativeJSEvalInstruction jsEvalInstruction)
    {
      if (instruction is IInstructionRequiresJsEngine jsEngineRequired)
      {
        jsEngineRequired.SetEngineGenerator(jsEngineGetter);
      }

      return new NativeJSEvalDefinition(jsEvalInstruction.ToEvalFunction(), jsEngineGetter);
    }
    else
    {
      throw new InvalidOperationException($"Invalid instruction type: {instruction.GetType().Name}.");
    }
  }
}
