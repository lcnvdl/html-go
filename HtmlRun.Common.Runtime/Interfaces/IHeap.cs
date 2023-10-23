using HtmlRun.Common.Models;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Interfaces;

public interface IHeap
{
  List<IHeapItem> ItemsRef { get; }

  int Alloc(object? val, Dictionary<string, object?>? attributes = null);

  IHeapItem Read(int ptr) => this.ItemsRef.First(m => m.Index == ptr);
}