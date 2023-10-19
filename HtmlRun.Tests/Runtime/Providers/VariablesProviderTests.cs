using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;

public class VariablesProviderTests : BaseProviderTests
{
  public VariablesProviderTests() : base(new VariablesProvider())
  {
  }

  [Fact]
  public void VariablesProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void VariablesProvider_Swap_ShouldWorkFine()
  {
    var instruction = this.GetInstruction(BasicInstructionsSet.Swap);
    Assert.NotNull(instruction);

    this.Ctx.DeclareVariable("Var1");
    this.Ctx.DeclareVariable("Var2");

    this.Ctx.SetValueVariable("Var1", "1");
    this.Ctx.SetValueVariable("Var2", "2");

    Assert.Equal("1", this.Ctx.GetVariable("Var1")!.Value);
    Assert.Equal("2", this.Ctx.GetVariable("Var2")!.Value);

    instruction.Action.Invoke(this.Ctx.Fork(this.Runtime, instruction.Key, new[] { ParsedArgument.String("Var1"), ParsedArgument.String("Var2") }));

    Assert.Equal("2", this.Ctx.GetVariable("Var1")!.Value);
    Assert.Equal("1", this.Ctx.GetVariable("Var2")!.Value);
  }
}