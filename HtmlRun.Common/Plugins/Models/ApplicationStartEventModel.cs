using System.Collections;
using System.Reflection;
using HtmlRun.Common.Models;

namespace HtmlRun.Common.Plugins.Models;

public class ApplicationStartEventModel
{
  public IDictionary EnvironmentVariables { get; private set; }

  public ApplicationStartEventModel(IDictionary env)
  {
    this.EnvironmentVariables = env;
  }
}

