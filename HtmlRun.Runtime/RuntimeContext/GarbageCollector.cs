namespace HtmlRun.Runtime;

public static class GarbageCollector
{
  public static void CollectFromReleasedContext(Context ctx)
  {
    if (ctx.Parent == null)
    {
      return;
    }

    var allVariables = ctx.AllHierarchyVariables.Where(m => m.Value != null);

    var toDispose = ctx.Heap.ItemsRef.Where(item => !allVariables.Any(variable => variable.Value == item.Index.ToString()));

    foreach (var item in toDispose)
    {
      ctx.Heap.ItemsRef.Remove(item);
    }
  }
}