using HtmlRun.Interfaces;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class DateProvider : INativeProvider
{
  public static IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();

  public string Namespace => "Date";

  public INativeInstruction[] Instructions => new INativeInstruction[] {
    new TimestampCmd(),
    new TimestampInSecondsCmd(),
  };
}

class DateTimeProvider : IDateTimeProvider
{
  public DateTime UtcNow => DateTime.UtcNow;
}

class TimestampCmd : INativeInstruction, INativeJSInstruction
{
  private static long Value => new DateTimeOffset(DateProvider.DateTimeProvider.UtcNow).ToUnixTimeMilliseconds();

  public string Key => Constants.DateInstructionsSet.Timestamp;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string>(() => Value.ToString());
  }
}

class TimestampInSecondsCmd : INativeInstruction, INativeJSInstruction
{
  private static long Value => new DateTimeOffset(DateProvider.DateTimeProvider.UtcNow).ToUnixTimeSeconds();

  public string Key => Constants.DateInstructionsSet.TimestampInSeconds;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx => { };
    }
  }

  public Delegate ToJSAction()
  {
    return new Func<string>(() => Value.ToString());
  }
}
