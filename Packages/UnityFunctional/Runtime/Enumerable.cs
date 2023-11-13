using System;
using System.Collections.Generic;
using System.Linq;

namespace Bravasoft.Functional
{
    using static Core;
    public static class Enumerable
    {
        public static U Match<T, U>(this IEnumerable<T> ts, Func<T, IEnumerable<T>, U> onAny, Func<U> onNone) =>
            ts.Any() ? onAny(ts.First(), ts.Skip(1)) : onNone();

        public static IEnumerable<T> Append<T>(this IEnumerable<T> ts, in Option<T> option) =>
            option.TryGetValue(out var some) ? System.Linq.Enumerable.Append(ts, some) : ts;

        public static IEnumerable<T> Somes<T>(this IEnumerable<Option<T>> options) =>
            options.Where(o => o.IsSome).Select(o =>
            {
                var (isSome, value) = o;
                return value;
            });

        public static IEnumerable<T> OKs<T>(this IEnumerable<Result<T>> results) =>
            results.Where(r => r.IsOk).Select(r =>
            {
                var (isOk, value, error) = r;
                return value;
            });

        public static Option<TSource> TrySingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            TSource ret = default;
            bool foundAny = false;

            foreach (TSource item in source)
            {
                if (predicate(item))
                {
                    if (foundAny)
                    {
                        return Option<TSource>.None;
                    }
                    foundAny = true;
                    ret = item;
                }
            }
            if (!foundAny)
            {
                return Option<TSource>.None;
            }
            return ret;
        }

        public static Option<TSource> TrySingle<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (IEnumerator<TSource> iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    return None;
                }
                TSource ret = iterator.Current;
                if (iterator.MoveNext())
                {
                    return None;
                }
                return ret;
            }
        }

        public static Option<TSource> TryFirst<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (IEnumerator<TSource> iterator = source.GetEnumerator())
            {
                return iterator.MoveNext() ? Prelude.Some(iterator.Current) : Option<TSource>.None;
            }
        }

        public static Option<TSource> TryFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            var enumerator = source.GetEnumerator();

            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            return None;
        }

        public static IEnumerable<(T Value, int Index)> Indexed<T>(this IEnumerable<T> ts) =>
            new IndexedEnumerator<T>(ts.GetEnumerator());

        public static Option<IEnumerable<UValue>> Traverse<TValue, UValue>(this IEnumerable<TValue> values, Func<TValue, Option<UValue>> f)
        {
            var seed = Option<IEnumerable<UValue>>.Some(System.Linq.Enumerable.Empty<UValue>());
            return values.Aggregate(seed: seed, (acc, r) => from x in f(r)
                                                            from acc1 in acc
                                                            select acc1.Append(x));
        }

        public static Result<IEnumerable<U>> Traverse<T, U>(this IEnumerable<T> values, Func<T, Result<U>> f)
        {
            var seed = Result<IEnumerable<U>>.Ok(System.Linq.Enumerable.Empty<U>());
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
