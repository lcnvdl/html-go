namespace HtmlRun.Common.Models;

public class ImportedLibraryModel
{
  public AppModel Origin { get; set; }

  public string Path { get; set; }

  public string? Alias { get; set; }

  public AppModel Library { get; set; }
}

