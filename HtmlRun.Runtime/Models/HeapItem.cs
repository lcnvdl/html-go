using HtmlRun.Runtime.Interfaces;

namespace HtmlRun.Runtime.Models;

public class HeapItem : IHeapItem
{
  public int Index { get; set; }

  public object? Data { get; set; }

  public Dictionary<string, object?>? AdditionalData { get; set; }

  public HeapItem(int index, object? data, Dictionary<string, object?>? additionalData)
  {
    this.Index = index;
    this.Data = data;
    this.AdditionalData = additionalData;
  }
}