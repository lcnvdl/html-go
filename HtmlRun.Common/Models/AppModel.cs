namespace HtmlRun.Common.Models;

public class AppModel
{
  public string Title { get; set; } = string.Empty;

  public List<InstructionsGroup> InstructionGroups { get; set; } = new();

  public List<FunctionModel> Functions { get; set; } = new();

  public List<EntityModel> Entities { get; set; } = new();
}

