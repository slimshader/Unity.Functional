using System.Collections.Generic;
using System.Linq;

namespace Bravasoft.Functional
{
    public static class CollectionsGeneric
    {
        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? Option.Some(value) : Option.None;

        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var value) ? Option.Some(value) : Option.None;

        public static Option<IEnumerable<T>> OptionalNonEmpty<T>(this IEnumerable<T> enumerable)
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

        public static Option<List<T>> OptionalNonEmpty<T>(this List<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);
        public static Option<IList<T>> OptionalNonEmpty<T>(this IList<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);
        public static Option<ICollection<T>> OptionalNonEmpty<T>(this ICollection<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);
        public static Option<IReadOnlyList<T>> OptionalNonEmpty<T>(this IReadOnlyList<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);
        public static Option<IReadOnlyCollection<T>> OptionalNonEmpty<T>(this IReadOnlyCollection<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);
    }
}
