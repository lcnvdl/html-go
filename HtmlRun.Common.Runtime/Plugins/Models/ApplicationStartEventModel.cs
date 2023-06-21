namespace HtmlRun.Common.Plugins.Models;

public class ApplicationStartEventModel
{
  public IDictionary<string, string> EnvironmentVariables { get; private set; }

  public ApplicationStartEventModel(IDictionary<string, string> env)
  {
    this.EnvironmentVariables = env;
  }
}

