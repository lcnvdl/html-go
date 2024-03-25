namespace HtmlRun.Interfaces;

public interface IDateTimeProvider
{
  DateTime UtcNow { get; }
}