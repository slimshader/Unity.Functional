using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    using static Prelude;

    public static class List
    {
        public static Option<T> TryFind<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < list.Count; ++i)
            {
                if (predicate(list[i]))
                {
                    return list[i];
                }
            }

            return none;
        }

        public static Option<int> TryFindIndex<T>(this List<T> list, Predicate<T> predicate)
        {
            var idx = list.FindIndex(predicate);
            return idx == -1 ? none : idx;
        }
    }
}
