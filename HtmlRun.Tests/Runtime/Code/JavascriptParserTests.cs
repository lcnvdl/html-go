using HtmlRun.Runtime.Code;

public class JavascriptParserTests
{
  [Fact]
  public void JavascriptParserTests_SimpleSolve_ShouldWorkFine()
  {
    var result = JavascriptParser.SimpleSolve("1+1").ToString();
    Assert.Equal("2", result);
  }
}