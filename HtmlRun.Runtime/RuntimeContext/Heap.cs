using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.Models;

namespace HtmlRun.Runtime;

public class Heap : IHeap
{
  private int nextIndex = 0;

  private readonly List<IHeapItem> items = new();

  public List<IHeapItem> ItemsRef => this.items;

  public int Alloc(object? val, Dictionary<string, object?>? attributes = null)
  {
    var heap = new HeapItem(Interlocked.Increment(ref this.nextIndex) - 1, val, attributes);
    this.items.Add(heap);
    return heap.Index;
  }
}