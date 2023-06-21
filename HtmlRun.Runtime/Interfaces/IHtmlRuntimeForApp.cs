using HtmlRun.Common.Models;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime;

public interface IHtmlRuntimeForApp
{
  void Run(AppModel app, CancellationToken? token);

  void RegisterProvider(INativeProvider provider);
}