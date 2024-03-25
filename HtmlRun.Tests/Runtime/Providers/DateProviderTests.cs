using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;
using HtmlRun.Tests.Stubs;

public class DateProviderTests : BaseProviderTests
{
  public DateProviderTests() : base(new DateProvider())
  {
    DateProvider.DateTimeProvider = new DateTimeProviderStub();
  }

  [Fact]
  public void DateProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void DateProvider_Timestamp_ShouldWorkFine()
  {
    long result = Convert.ToInt64(this.CallJsInstruction(DateInstructionsSet.Timestamp)?.ToString());
    Assert.Equal(1704078000000L, result);
  }

  [Fact]
  public void DateProvider_TimestampInSeconds_ShouldWorkFine()
  {
    long result = Convert.ToInt64(this.CallJsInstruction(DateInstructionsSet.TimestampInSeconds)?.ToString());
    Assert.Equal(1704078000L, result);
  }
}
