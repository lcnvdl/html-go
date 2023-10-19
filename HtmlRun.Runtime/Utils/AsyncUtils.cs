namespace HtmlRun.Runtime.Utils;

public static class AsyncUtils
{
  public static T ToSync<T>(Func<Task<T>> asyncFunction)
  {
    try
    {
      Task<T> task = Task.Run(async () => await asyncFunction());
      return task.Result;
    }
    catch (AggregateException ex)
    {
      throw ex.InnerException ?? ex;
    }
  }
}