using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class ImportsProviderTests : BaseProviderTests
{
  public ImportsProviderTests() : base(new ImportsProvider())
  {
  }

  [Fact]
  public void ImportsProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void ImportsProvider_Using_ShouldWorkFine()
  {
    Assert.DoesNotContain("Threading", this.Ctx.Usings);

    var instruction = this.GetInstruction(BasicInstructionsSet.Using);
    Assert.NotNull(instruction);
    instruction.Action.Invoke(this.Ctx.Fork(this.Runtime, instruction.Key, new[] { ParsedArgument.String("Threading") }));

    Assert.Contains("Threading", this.Ctx.Usings);
  }

  [Fact]
  public void ImportsProvider_Using_ShouldFailIfNamespaceDoesNotExists()
  {
    var instruction = this.GetInstruction(BasicInstructionsSet.Using);
    Assert.NotNull(instruction);
    Assert.Throws<Exception>(() => instruction.Action.Invoke(this.Ctx.Fork(this.Runtime, instruction.Key, new[] { ParsedArgument.String("Threadinggg") })));
  }
}