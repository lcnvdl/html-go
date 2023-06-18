using Jurassic.Library;

namespace HtmlRun.Runtime.Utils;

public static class JurassicUtils
{
  public static Dictionary<string, object?> ToDictionary(ObjectInstance instance)
  {
    var dict = new Dictionary<string, object?>();

    foreach (var pi in instance.Properties)
    {
      try
      {
        dict[pi.Key.ToString()!] = pi.Value;
        // var value = instance.GetPropertyValue(new PropertyReference(pi.Name));
        // if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
        // {
        //   pi.SetValue(ret, Convert.ChangeType(value, pi.PropertyType), null);
        // }
      }
      catch { }
    }

    return dict;
  }

  public static T? ToObject<T>(ObjectInstance? instance) where T : new()
  {
    if (instance == null)
    {
      return default;
    }

    var ret = new T();

    foreach (var pi in ret.GetType().GetProperties())
    {
      try
      {
        var value = instance.GetPropertyValue(new PropertyReference(pi.Name));
        if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
        {
          pi.SetValue(ret, Convert.ChangeType(value, pi.PropertyType), null);
        }
      }
      catch { }
    }

    return ret;
  }
}