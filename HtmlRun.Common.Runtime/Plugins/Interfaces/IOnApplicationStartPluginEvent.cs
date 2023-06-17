using HtmlRun.Common.Plugins.Models;

namespace HtmlRun.Common.Plugins;

public interface IOnApplicationStartPluginEvent
{
  void OnApplicationStart(ApplicationStartEventModel data);
}