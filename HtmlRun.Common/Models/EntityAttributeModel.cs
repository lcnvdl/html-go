namespace HtmlRun.Common.Models;

public class EntityAttributeModel
{
  public string Name { get; set; } = "";

  public string SqlType { get; set; } = "INTEGER";

  public bool IsNull { get; set; } = false;

  public bool IsPK { get; set; } = false;

  public int Length { get; set; } = 0;
  
  public string? DefaultValue { get; set; }
}

