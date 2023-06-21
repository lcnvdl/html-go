namespace HtmlRun.Common.Models;

public class EntityModel
{
  public string Name { get; set; } = "";

  public List<EntityAttributeModel> Attributes { get; set; } = new();
}

