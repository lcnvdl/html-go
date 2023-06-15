using HtmlRun.Common.Models;

namespace HtmlRun.Common.Plugins;

public interface IOnLoadEntityPluginEvent
{
  void OnLoadEntity(EntityModel entity);
}