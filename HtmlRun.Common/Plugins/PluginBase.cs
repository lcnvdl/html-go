using System.Reflection;
using HtmlRun.Common.Models;

namespace HtmlRun.Common.Plugins;

public abstract class PluginBase
{
  protected Assembly AppAssembly { get; private set; }

  public PluginBase(Assembly appAssembly)
  {
    this.AppAssembly = appAssembly;
  }
}

