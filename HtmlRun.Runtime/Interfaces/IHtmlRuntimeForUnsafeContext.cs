using HtmlRun.Common.Plugins;

namespace HtmlRun.Runtime.Interfaces;

public interface IHtmlRuntimeForUnsafeContext
{
  void RegisterPlugin(PluginBase plugin);
}
