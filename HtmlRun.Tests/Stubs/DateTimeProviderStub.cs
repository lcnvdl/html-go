using HtmlRun.Interfaces;

namespace HtmlRun.Tests.Stubs;

public class DateTimeProviderStub : IDateTimeProvider
{
  public DateTime UtcNow { get; set; } = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
}