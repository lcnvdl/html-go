namespace HtmlRun.Common.Models;

public class EntityAttributeModel
{
  public string Name { get; set; } = "";

  public string SqlType { get; set; } = "INTEGER";

  public string? DefaultValue { get; set; }
}

