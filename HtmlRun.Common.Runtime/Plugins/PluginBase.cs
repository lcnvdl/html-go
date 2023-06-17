using System.Reflection;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Common.Plugins;

public abstract class PluginBase
{
  protected Assembly AppAssembly { get; private set; }

  public virtual INativeProvider[]? Providers => null;

  public virtual string Name => this.GetType().FullName?.Replace(".", "") ?? "Unknown";

  public PluginBase(Assembly appAssembly)
  {
    this.AppAssembly = appAssembly;
  }
}

