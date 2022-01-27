using System;
using System.Collections.Generic;

namespace Bravasoft.Unity.Functional
{
    public static class ValueTupleExtensions
    {
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
