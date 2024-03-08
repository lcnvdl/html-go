using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;

public class ListProviderTests : BaseProviderTests
{
  public ListProviderTests() : base(new ListProvider())
  {
  }

  [Fact]
  public void ListProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void ListProvider_GetSize_JS_ShouldWorkFine()
  {
    int result = (int)(this.CallJsInstruction(ListInstructionsSet.GetSize, "[]") ?? throw new NullReferenceException());
    Assert.Equal(0, result);
  }

  [Fact]
  public void ListProvider_GetSizeWithStringItems_JS_ShouldWorkFine()
  {
    int result = (int)(this.CallJsInstruction(ListInstructionsSet.GetSize, "[\"1\",\"2\",\"3\"]") ?? throw new NullReferenceException());
    Assert.Equal(3, result);
  }

  [Fact]
  public void ListProvider_GetSizeWithNumericItems_JS_ShouldWorkFine()
  {
    int result = (int)(this.CallJsInstruction(ListInstructionsSet.GetSize, "[1,2,3]") ?? throw new NullReferenceException());
    Assert.Equal(3, result);
  }
}
