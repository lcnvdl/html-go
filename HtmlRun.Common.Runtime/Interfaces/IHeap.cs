using HtmlRun.Common.Models;
using HtmlRun.Runtime.Native;

namespace HtmlRun.Runtime.Interfaces;

public interface IHeap
{
  List<IHeapItem> ItemsRef { get; }

  int Alloc(object? val, Dictionary<string, object?>? attributes = null);
}