using HtmlRun.Interfaces;
using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.RuntimeContext;

class JumpReturn : IContextJump, IJumpReturn
{
  public bool IsEmpty => false;

  public object Clone()
  {
    return base.MemberwiseClone();
  }
}