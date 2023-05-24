using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Option<IEnumerable<T>> TryNotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return Option.None;
            }
            /* If this is a list, use the Count property. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1 ? Option.None : Option.Some(enumerable);
            }

            return !enumerable.Any() ? Option.None : Option.Some(enumerable);
        }

        public static Option<ICollection<T>> IsNullOrEmpty<T>(this ICollection<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);

        public static Option<IReadOnlyList<T>> IsNullOrEmpty<T>(this IReadOnlyList<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);

        public static Option<IReadOnlyCollection<T>> IsNullOrEmpty<T>(this IReadOnlyCollection<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);
    }
}
