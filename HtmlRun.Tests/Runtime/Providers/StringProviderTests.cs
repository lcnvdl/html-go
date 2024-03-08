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
  public void StringProvider_Concat_JS_ShouldWorkFine()
  {
    string? result = this.CallEvalInstruction(StringInstructionsSet.Concat, "'hi there'")?.ToString();
    Assert.Equal("hi there", result);
  }

  [Fact]
  public void StringProvider_Concat_TwoArguments_JS_ShouldWorkFine()
  {
    string? result = this.CallEvalInstruction(StringInstructionsSet.Concat, "1", "2")?.ToString();
    Assert.Equal("12", result);
  }

  [Fact]
  public void StringProvider_Concat_ThreeArguments_JS_ShouldWorkFine()
  {
    string? result = this.CallEvalInstruction(StringInstructionsSet.Concat, "'hi'", "' '", "'world'")?.ToString();
    Assert.Equal("hi world", result);
  }

  [Fact]
  public void StringProvider_Join_JS_ShouldWorkFine()
  {
    string? result = this.CallEvalInstruction(StringInstructionsSet.Join, "', '", "['hi', 'there']")?.ToString();

    Assert.Equal("hi, there", result);
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
}
