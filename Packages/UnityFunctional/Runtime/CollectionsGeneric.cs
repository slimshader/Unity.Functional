using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public static class CollectionsGeneric
    {
        public static Option<T> TryFirst<T>(this List<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; ++i)
                if (predicate(list[i]))
                    return list[i];

            return Option<T>.None;
        }

        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? Option.Some(value) : Option.None;

        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? Option.Some(value) : Option.None;
    }
}
