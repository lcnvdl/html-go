namespace HtmlRun.Common.Models;

public class AppModel
{
  public string Title { get; set; } = string.Empty;

  public string Version { get; set; } = string.Empty;

  public AppType Type { get; set; } = AppType.Unknown;

  public List<InstructionsGroup> InstructionGroups { get; set; } = new();

  public List<FunctionModel> Functions { get; set; } = new();

  public List<EntityModel> Entities { get; set; } = new();

  public List<ImportedLibraryModel> Imports { get; set; } = new();
}

