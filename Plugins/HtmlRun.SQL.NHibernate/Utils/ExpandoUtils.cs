using System.Collections;
using System.Dynamic;

namespace HtmlRun.SQL.NHibernate.Utils;

class ExpandoUtils
{
  public static ExpandoObject ToExpandoWithNullableValues(IDictionary<string, object?> dictionary)
  {
    ExpandoObject expando = new();
    IDictionary<string, object?> expandoDic = expando!;

    // go through the items in the dictionary and copy over the key value pairs)
    foreach (var kvp in dictionary)
    {
      // if the value can also be turned into an ExpandoObject, then do it!
      if (kvp.Value is IDictionary<string, object?>)
      {
        var expandoValue = ToExpandoWithNullableValues((IDictionary<string, object?>)kvp.Value);
        expandoDic.Add(kvp.Key, expandoValue);
      }
      else if (kvp.Value is ICollection)
      {
        // iterate through the collection and convert any strin-object dictionaries
        // along the way into expando objects
        var itemList = new List<object>();
        foreach (var item in (ICollection)kvp.Value)
        {
          if (item is IDictionary<string, object?>)
          {
            var expandoItem = ToExpandoWithNullableValues((IDictionary<string, object?>)item);
            itemList.Add(expandoItem);
          }
          else
          {
            itemList.Add(item);
          }
        }

        expandoDic.Add(kvp.Key, itemList);
      }
      else
      {
        expandoDic.Add(kvp);
      }
    }

    return expando;
  }

  public static ExpandoObject ToExpando(IDictionary<string, object> dictionary)
  {
    ExpandoObject expando = new();
    IDictionary<string, object> expandoDic = expando!;

    // go through the items in the dictionary and copy over the key value pairs)
    foreach (var kvp in dictionary)
    {
      // if the value can also be turned into an ExpandoObject, then do it!
      if (kvp.Value is IDictionary<string, object>)
      {
        var expandoValue = ToExpando((IDictionary<string, object>)kvp.Value);
        expandoDic.Add(kvp.Key, expandoValue);
      }
      else if (kvp.Value is ICollection)
      {
        // iterate through the collection and convert any strin-object dictionaries
        // along the way into expando objects
        var itemList = new List<object>();
        foreach (var item in (ICollection)kvp.Value)
        {
          if (item is IDictionary<string, object>)
          {
            var expandoItem = ToExpando((IDictionary<string, object>)item);
            itemList.Add(expandoItem);
          }
          else
          {
            itemList.Add(item);
          }
        }

        expandoDic.Add(kvp.Key, itemList);
      }
      else
      {
        expandoDic.Add(kvp);
      }
    }

    return expando;
  }
}