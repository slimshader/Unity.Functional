using System.Collections.Generic;

namespace Bravasoft.Functional
{
    using static Prelude;

    public static class CollectionsGeneric
    {
        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : none;

        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? value : none;
    }
}
