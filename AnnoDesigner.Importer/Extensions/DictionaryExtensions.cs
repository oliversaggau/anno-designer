using System.Collections.Generic;

namespace AnnoDesigner.Importer
{
    internal static class DictionaryExtensions
    {
        public static TValue? TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : null;
        }
    }
}
