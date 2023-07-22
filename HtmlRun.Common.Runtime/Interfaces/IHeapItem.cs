namespace HtmlRun.Runtime.Interfaces;

public interface IHeapItem
{
  int Index { get; set; }

  object? Data { get; set; }

  Dictionary<string, object?>? AdditionalData { get; set; }
}