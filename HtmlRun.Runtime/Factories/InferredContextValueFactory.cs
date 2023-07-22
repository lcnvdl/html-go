using HtmlRun.Runtime.Interfaces;
using HtmlRun.Runtime.RuntimeContext;

namespace HtmlRun.Runtime.Factories;

static class InferredContextValueFactory
{
  public static IEnumerable<ContextValue> InferValuesFromHeapItem(IHeapItem heapItem)
  {
    if (heapItem.Data == null)
    {
      return new List<ContextValue>();
    }

    if (heapItem.AdditionalData != null)
    {
      return InferValuesFromDictionaryHeapObject(heapItem.AdditionalData);
    }
    else
    {
      return InferValuesFromGenericHeapObjectRecursively(heapItem.Data);
    }
  }

  private static IEnumerable<ContextValue> InferValuesFromDictionaryHeapObject(Dictionary<string, object?> additionalData)
  {
    foreach (var kv in additionalData)
    {
      yield return new DictionaryInferredContextValue(additionalData, kv.Key);
    }
  }

  private static IEnumerable<ContextValue> InferValuesFromGenericHeapObjectRecursively(object heapObject, string? prefix = null)
  {
    var type = heapObject.GetType();

    foreach (var field in type.GetFields())
    {
      var value = field.GetValue(heapObject);

      var name = string.IsNullOrEmpty(prefix) ? field.Name : $"{prefix}.{field.Name}";

      if (value == null || field.FieldType.IsPrimitive || field.FieldType == typeof(string))
      {
        yield return new ReflectionInferredContextValue(heapObject, name);
      }
      else
      {
        foreach (var item in InferValuesFromGenericHeapObjectRecursively(value, name))
        {
          yield return item;
        }
      }
    }
  }
}
