namespace HtmlRun.SQL.NHibernate;

public sealed class PluginSettings
{
  public string DatabaseLibrary { get; set; } = "";

  public string? DatabaseLibraryVersion { get; set; }

  public string? ConnectionString { get; set; }

  public bool TestDatabaseAfterConnection { get; set; } = false;

  public void Load(IDictionary<string, string> environment)
  {
    this.ConnectionString = environment.GetOrDefault(nameof(ConnectionString));

    this.DatabaseLibrary = (environment.GetOrDefault(nameof(DatabaseLibrary)) ?? Constants.DatabaseEngines.SQLite).ToLower();

    this.DatabaseLibraryVersion = environment.GetOrDefault(nameof(DatabaseLibraryVersion));

    this.TestDatabaseAfterConnection = environment.GetOrDefault(nameof(TestDatabaseAfterConnection)) == "true";
  }
}