using System.Collections;

namespace System.Linq;

#pragma warning disable 8714

public static class IDictionaryExtension
{
  public static T2 GetOrDefault<T1, T2>(this IDictionary<T1, T2> dictionary, T1 key)
  {
    if (dictionary.TryGetValue(key, out var result))
    {
      return result;
    }

    return default!;
  }

  public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IDictionary x)
  {
    var keys = x.Keys
    .Cast<object>()
    .Select(m => new KeyValuePair<T1, T2>(
      (T1)Convert.ChangeType(m, typeof(T1))!,
      (T2)Convert.ChangeType(x[m], typeof(T2))!));

    return new Dictionary<T1, T2>(keys);
  }
}

#pragma warning restore 8714
