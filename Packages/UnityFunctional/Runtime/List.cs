using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public static class List
    {
        public static Option<List<T>> OptionalNonEmpty<T>(this List<T> collection) =>
            collection == null || collection.Count < 1 ? Option.None : Option.Some(collection);

        public static Option<T> TryFind<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < list.Count; ++i)
            {
                if (predicate(list[i]))
                {
                    return Option.Some(list[i]);
                }
            }

            return Option.None;
        }

        public static Option<int> TryFindIndex<T>(this List<T> list, Predicate<T> predicate)
        {
            var idx = list.FindIndex(predicate);
            return idx == -1 ? Option.None : Option.Some(idx);
        }
    }
}
