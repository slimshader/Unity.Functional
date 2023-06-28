using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public static class ValueTuple
    {
        public static Option<ValueTuple<T1, T2>> Traverse<T1, T2>(this in ValueTuple<Option<T1>, Option<T2>> tuple) =>
            tuple.Item1.TryGetValue(out var v1) &&
            tuple.Item2.TryGetValue(out var v2)
            ? Option.Some((v1, v2)) : Option.None;

        public static Option<ValueTuple<T1, T2, T3>> Traverse<T1, T2, T3>(this in ValueTuple<Option<T1>, Option<T2>, Option<T3>> tuple) =>
            tuple.Item1.TryGetValue(out var v1) &&
            tuple.Item2.TryGetValue(out var v2) &&
            tuple.Item3.TryGetValue(out var v3)
            ? Option.Some((v1, v2, v3)) : Option.None;


        public static ValueTuple<T1, T2> Append<T1, T2>(this in SingleValue<T1> single, T2 value) =>
            (single.Value, value);

        public static ValueTuple<T1, T2, T3> Append<T1, T2, T3>(this in ValueTuple<T1, T2> tuple, T3 value) =>
            (tuple.Item1, tuple.Item2, value);

        public static ValueTuple<T1, T2, T3, T4> Append<T1, T2, T3, T4>(this in ValueTuple<T1, T2, T3> tuple, T4 value) =>
            (tuple.Item1, tuple.Item2, tuple.Item3, value);

        public static IEnumerable<T> ToEnumerable<T>(this ValueTuple<T, T> tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
        }

        public static IEnumerable<T> ToEnumerable<T>(this ValueTuple<T, T, T> tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
            yield return tuple.Item3;
        }

        public static IEnumerable<T> ToEnumerable<T>(this ValueTuple<T, T, T, T> tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
            yield return tuple.Item3;
            yield return tuple.Item4;
        }
    }
}
