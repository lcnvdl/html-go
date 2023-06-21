namespace HtmlRun.Runtime.Utils;

public static class EnvironmentUtils
{
  public static bool IsDevelopment
  {
    get => Environment.GetEnvironmentVariable("ENVIRONMENT_NAME") == "DEV";
    set => Environment.SetEnvironmentVariable("ENVIRONMENT_NAME", value ? "DEV" : "PROD");
  }
}
