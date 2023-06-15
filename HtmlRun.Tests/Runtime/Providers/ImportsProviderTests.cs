using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class ImportsProviderTests : BaseProviderTests
{
  private Context ctx;
  private HtmlRuntime runtime;

  public ImportsProviderTests() : base(new ImportsProvider())
  {
    this.ctx = new Context(null, new Stack<Context>());
    this.runtime = new HtmlRuntime();
    this.runtime.RegisterProvider(new InstructionsProvider());
    this.runtime.RegisterBasicProviders();
    LogCmd.Logs.Clear();
  }

  [Fact]
  public void ImportsProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void ImportsProvider_Using_ShouldWorkFine()
  {
    Assert.DoesNotContain("Threading", this.ctx.Usings);

    var instruction = this.GetInstruction(BasicInstructionsSet.Using);
    Assert.NotNull(instruction);
    instruction.Action.Invoke(this.ctx.Fork(this.runtime, instruction.Key, new[] { ParsedArgument.String("Threading") }));

    Assert.Contains("Threading", this.ctx.Usings);
  }

  [Fact]
  public void ImportsProvider_Using_ShouldFailIfNamespaceDoesNotExists()
  {
    var instruction = this.GetInstruction(BasicInstructionsSet.Using);
    Assert.NotNull(instruction);
    Assert.Throws<Exception>(() => instruction.Action.Invoke(this.ctx.Fork(this.runtime, instruction.Key, new[] { ParsedArgument.String("Threadinggg") })));
  }
}