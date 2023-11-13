using System.Collections.Generic;
using static Bravasoft.Functional.Prelude;

namespace Bravasoft.Functional
{
    public static class CollectionsGeneric
    {
        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : None;

        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : None;
    }
}
