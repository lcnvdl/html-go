namespace HtmlRun.Common.Models;

public sealed class InstructionGroupArgumentModel
{
  public string Name { get; set; }

  public InstructionGroupArgumentModel(string name)
  {
    this.Name = name;
  }
}