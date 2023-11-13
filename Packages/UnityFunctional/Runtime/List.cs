using System;
using System.Collections.Generic;
using static Bravasoft.Functional.Core;

namespace Bravasoft.Functional
{
    public static class List
    {
        public static Option<T> TryFind<T>(this List<T> list, Func<T, bool> predicate)
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

            return None;
        }

        public static Option<int> TryFindIndex<T>(this List<T> list, Predicate<T> predicate)
        {
            var idx = list.FindIndex(predicate);
            return idx == -1 ? None : idx;
        }
    }
}
