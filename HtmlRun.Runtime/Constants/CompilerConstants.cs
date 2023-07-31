using HtmlRun.Common.Models;

namespace HtmlRun.Runtime.Constants;

public static class CompilerConstants
{
  public static string GroupStartLabel(AppModel app, InstructionsGroup group) => GroupStartLabel(app.Id, group.Label);

  public static string GroupStartLabel(string appId, string groupLabel) => $"group-{appId}-{groupLabel}";

  public static string GroupEndLabel(AppModel app, InstructionsGroup group) => GroupEndLabel(app.Id, group.Label);

  public static string GroupEndLabel(string appId, string groupLabel) => $"end-group-{appId}-{groupLabel}";

  public static string ApplicationEndLabel(AppModel app) => ApplicationEndLabel(app.Id);

  public static string ApplicationEndLabel(string appId) => $"application-end-{appId}";
}