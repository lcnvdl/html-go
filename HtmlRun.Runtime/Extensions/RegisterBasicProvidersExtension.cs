using HtmlRun.Runtime.Providers;

namespace HtmlRun.Runtime;

public static class RegisterBasicProvidersExtension
{
  public static void RegisterBasicProviders(this HtmlRuntime runtime)
  {
    //  Language instructions
    runtime.RegisterProvider(new ConditionalProvider());
    runtime.RegisterProvider(new GotoProvider());
    runtime.RegisterProvider(new ImportsProvider());
    runtime.RegisterProvider(new VariablesProvider());
    runtime.RegisterProvider(new OOPProvider());

    //  Plugins
    runtime.RegisterProvider(new PluginsProvider());

    //  Tools
    runtime.RegisterProvider(new EnvironmentProvider());
    runtime.RegisterProvider(new DateProvider());

    //  TODO  Threading is not a basic provider
    runtime.RegisterProvider(new ThreadingProvider());
  }
}