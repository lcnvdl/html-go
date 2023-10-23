using System.Text.Json;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;
using Jurassic.Library;

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

    var arrResult = result as ArrayInstance;
    Assert.NotNull(arrResult);

    string json = JsonSerializer.Serialize(arrResult!.ElementValues.ToArray());
    Assert.Equal("[\"hi\",\"dude!\"]", json);
  }

  [Fact]
  public void StringProvider_Join_JS_ShouldWorkFine()
  {
    var arr = this.JavascriptParserWithContext.GetInteropArray(new string[] { "hi", "dude" });

    object? result = this.CallJsInstruction(StringInstructionsSet.Join, arr, " ");
    Assert.NotNull(result);

    string text = result?.ToString()!;
    Assert.Equal("hi dude", text);
  }
}