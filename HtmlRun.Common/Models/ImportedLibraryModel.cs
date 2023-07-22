namespace HtmlRun.Common.Models;

public class ImportedLibraryModel
{
  public AppModel Origin { get; private set; }

  public string Path { get; private set; }

  public string? Alias { get; private set; }

  public AppModel Library { get; private set; }

  public ImportedLibraryModel(AppModel origin, string path, AppModel library, string? alias)
  {
    this.Origin = origin;
    this.Path = path;
    this.Alias = alias;
    this.Library = library;
  }
}

