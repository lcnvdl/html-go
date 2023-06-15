using HtmlRun.Common.Plugins.Models;

namespace HtmlRun.Common.Plugins;

public interface IBeforeApplicationLoadPluginEvent
{
  void BeforeApplicationLoad(ApplicationStartEventModel data);
}