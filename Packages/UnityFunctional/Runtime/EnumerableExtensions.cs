using System;
using System.Collections.Generic;
using System.Linq;

namespace Bravasoft.Unity.Functional
{
    public static class EnumerableExtensions
    {
        public static U Match<T, U>(this IEnumerable<T> ts, Func<T, IEnumerable<T>, U> onAny, Func<U> onNone) =>
            ts.Any() ? onAny(ts.First(), ts.Skip(1)) : onNone();

        public static IEnumerable<T> Append<T>(this IEnumerable<T> ts, in Option<T> option) =>
            option.TryGetSome(out var some) ? ts.Append(some) : ts;

        public static Option<T> FirstOrNone<T>(this IEnumerable<T> ts)
        {
            var enumerator = ts.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            return Option.None;
        }

        public static Option<T> FirstOrNone<T>(this IEnumerable<T> ts, Func<T, bool> predicate)
        {
            foreach (var value in ts.Where(predicate))
            {
                return Option.Some(value);
            }

            return Option.None;
        }

        public static Option<IEnumerable<UValue>> Traverse<TValue, UValue>(this IEnumerable<TValue> values, Func<TValue, Option<UValue>> f)
        {
            var seed = Option<IEnumerable<UValue>>.Some(Enumerable.Empty<UValue>());
            return values.Aggregate(seed: seed, (acc, r) => from x in f(r)
                                                            from acc1 in acc
                                                            select acc1.Append(x));
        }

        public static Result<IEnumerable<U>> Traverse<T, U>(this IEnumerable<T> values, Func<T, Result<U>> f)
        {
            var seed = Result<IEnumerable<U>>.Ok(Enumerable.Empty<U>());
            return values.Aggregate(seed: seed, (acc, r) => from x in f(r)
                                                            from acc1 in acc
                                                            select acc1.Append(x));
        }

        public static Result<U> Fold<T, U>(this IEnumerable<T> ts, Result<U> seed, Func<U, T, Result<U>> func) =>
            ts.Aggregate(seed, (acc, t) => from g in acc
                                           from r in func(g, t)
                                           select r);
    }
}
