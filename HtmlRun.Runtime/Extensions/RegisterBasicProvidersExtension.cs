using HtmlRun.Runtime.Providers;

namespace HtmlRun.Runtime;

public static class RegisterBasicProvidersExtension
{
  public static void RegisterBasicProviders(this IHtmlRuntimeForApp runtime)
  {
    //  Language instructions
    runtime.RegisterProvider(new SelectionStatementsProvider());
    runtime.RegisterProvider(new JumpStatementsProvider());
    runtime.RegisterProvider(new IterationStatementsProvider());
    runtime.RegisterProvider(new ImportsProvider());
    runtime.RegisterProvider(new VariablesProvider());
    runtime.RegisterProvider(new OOPProvider());

    //  Plugins
    runtime.RegisterProvider(new PluginsProvider());

    //  Types and tools
    runtime.RegisterProvider(new EnvironmentProvider());
    runtime.RegisterProvider(new DateProvider());
    runtime.RegisterProvider(new StringProvider());
    runtime.RegisterProvider(new HttpClientProvider());
    runtime.RegisterProvider(new ThreadingProvider());
  }
}