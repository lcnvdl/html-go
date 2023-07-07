using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Providers;

class DateProvider : INativeProvider
{
  public string Namespace => "Date";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new TimestampCmd(), new TimestampInSecondsCmd() };
}

class TimestampCmd : INativeInstruction, INativeJSInstruction
{
  private static long Value => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

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
  private static long Value => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

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
