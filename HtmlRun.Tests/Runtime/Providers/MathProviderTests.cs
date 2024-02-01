using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;

public class MathProviderTests : BaseProviderTests
{
  public MathProviderTests() : base(new MathProvider())
  {
  }

  [Fact]
  public void MathProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }
}