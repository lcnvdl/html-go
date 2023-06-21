using HtmlRun.Common.Plugins;
using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Native;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Providers;

class PluginsProvider : INativeProvider
{
  public string Namespace => "Plugins";

  public INativeInstruction[] Instructions => new INativeInstruction[] { new LoadPluginCmd(), };
}

class LoadPluginCmd : INativeInstruction
{
  public string Key => Constants.PluginsInstructionsSet.Load;

  public Action<ICurrentInstructionContext> Action
  {
    get
    {
      return ctx =>
      {
        string pluginName = ctx.GetRequiredArgument();
        var assembly = System.Reflection.Assembly.LoadFrom(pluginName);
        var pluginType = assembly.GetTypes().FirstOrDefault(m => typeof(PluginBase).IsAssignableFrom(m));

        if (pluginType == null)
        {
          throw new TypeAccessException($"Plugin class for {pluginName} not found.");
        }

        var instance = (PluginBase?)Activator.CreateInstance(pluginType, System.Reflection.Assembly.GetEntryAssembly()) ?? throw new TypeLoadException($"Error starting the plugin {pluginName}.");

        var unsafeCtx = (IUnsafeCurrentInstructionContext)ctx;
        unsafeCtx.UnsafeRuntime.RegisterPlugin(instance!);
      };
    }
  }
}
