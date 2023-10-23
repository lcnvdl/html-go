using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;

public class StringProviderTests : BaseProviderTests
{
  public StringProviderTests() : base(new StringProvider())
  {
  }

  [Fact]
  public void StringProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void StringProvider_Trim_JS_ShouldWorkFine()
  {
    string? result = this.CallJsInstruction(StringInstructionsSet.Trim, "hi ")?.ToString();
    Assert.Equal("hi", result);
  }

  [Fact]
  public void StringProvider_ToLowerCase_JS_ShouldWorkFine()
  {
    string? result = this.CallJsInstruction(StringInstructionsSet.ToLowerCase, "HI")?.ToString();
    Assert.Equal("hi", result);
  }

  [Fact]
  public void StringProvider_ToUpperCase_JS_ShouldWorkFine()
  {
    string? result = this.CallJsInstruction(StringInstructionsSet.ToUpperCase, "hi")?.ToString();
    Assert.Equal("HI", result);
  }

  [Fact]
  public void StringProvider_ToTitleCase_JS_ShouldWorkFine()
  {
    string? result = this.CallJsInstruction(StringInstructionsSet.ToTitleCase, "this is a title")?.ToString();
    Assert.Equal("This Is A Title", result);
  }

  [Fact]
  public void StringProvider_Split_JS_ShouldWorkFine()
  {
    object? result = this.CallJsInstruction(StringInstructionsSet.Split, "hi,dude!", ",");
    Assert.NotNull(result);

    string json = result?.ToString()!;
    Assert.Equal("[\"hi\",\"dude!\"]", json);
  }
}