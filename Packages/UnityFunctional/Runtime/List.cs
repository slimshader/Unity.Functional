﻿using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    using static Prelude;
    public static class List
    {
        public static IReadOnlyList<T> Create<T>(T v1) => new List<T> { v1 };
        public static IReadOnlyList<T> Create<T>(T v1, T v2) => new List<T> { v1, v2 };
        public static IReadOnlyList<T> Create<T>(T v1, T v2, T v3) => new List<T> { v1, v2, v3 };
        public static IReadOnlyList<T> Create<T>(T v1, T v2, T v3, T v4) => new List<T> { v1, v2, v3, v4 };
        public static IReadOnlyList<T> Create<T>(T v1, T v2, T v3, T v4, T v5) => new List<T> { v1, v2, v3, v4, v5 };

        public static Option<T> TryFirst<T>(this IReadOnlyList<T> list) =>
            list.Count >= 1 ? none : Some(list[0]);

        public static Option<T> TryFirst<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
        {
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

        public static Option<(T First, T Second)> TryFistPair<T>(this IReadOnlyList<T> list) =>
            list.Count >= 2 ? none : Some((list[0], list[1]));

        public static Option<T> TryFind<T>(this List<T> list, Predicate<T> predicate) => 
            list.Find(predicate);

        public static Option<int> TryFindIndex<T>(this List<T> list, Predicate<T> predicate)
        {
            var idx = list.FindIndex(predicate);
            return idx == -1 ? none : idx;
        }
    }
}
