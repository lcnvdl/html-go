namespace HtmlRun.Common.Models;

public class AppModel
{
  public string Id { get; set; } = Guid.NewGuid().ToString();

  public string Title { get; set; } = string.Empty;

  public string Version { get; set; } = string.Empty;

  public string? Alias { get; set; }

  public AppType Type { get; set; } = AppType.Unknown;

  public List<InstructionsGroup> InstructionGroups { get; set; } = new();

  public IEnumerable<InstructionsGroup> LabeledGroups => this.InstructionGroups.Where(m => !m.IsMain);

  public List<FunctionModel> Functions { get; set; } = new();

  public List<EntityModel> Entities { get; set; } = new();

  public List<ImportedLibraryModel> Imports { get; set; } = new();
}

