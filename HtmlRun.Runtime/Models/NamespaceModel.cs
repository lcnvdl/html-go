namespace HtmlRun.Runtime.Models;

public class NamespaceModel
{
  public string[] Parts { get; private set; }

  public NamespaceModel(string parts, bool ignoreLastPart)
  {
    if (ignoreLastPart)
    {
      this.Parts = parts.Remove(parts.LastIndexOf('.')).Split('.');
    }
    else
    {
      this.Parts = parts.Split('.');
    }
  }

  public NamespaceModel(string[] parts)
  {
    this.Parts = parts;
  }

  public override string ToString()
  {
    return string.Join('.', this.Parts);
  }
}