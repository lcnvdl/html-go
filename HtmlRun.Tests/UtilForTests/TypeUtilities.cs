using System.Reflection;

static class TypeUtilities
{
  public static List<FieldInfo> GetAllPublicConstantFields(Type type)
  {
    return type
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        .ToList();
  }

  public static List<T?> GetAllPublicConstantValues<T>(Type type)
  {
    return GetAllPublicConstantFields(type)
      .Select(m => m.GetRawConstantValue())
      .Cast<T?>()
      .ToList();
  }
}