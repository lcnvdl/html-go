using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.RuntimeContext;

public abstract class BaseContext
{
  public string UUID { get; private set; }

  public BaseContext()
  {
    this.UUID = Guid.NewGuid().ToString();
  }
}
