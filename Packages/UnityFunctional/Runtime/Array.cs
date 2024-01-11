namespace Bravasoft.Functional
{
    using System;
    using static Prelude;
    public static class Array
    {
        public static Option<T> TryFirst<T>(this T[] array) =>
            array.Length >= 1 ? Some(array[0]) : none;

        public static Option<T> TryFirst<T>(this T[] array, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < array.Length; ++i)
            {
                if (predicate(array[i]))
                {
                    return array[i];
                }
            }

            return none;
        }

        public static Option<(T First, T Second)> TryFirstPair<T>(this T[] array) =>
            array.Length >= 2 ? none : Some((array[0], array[1]));
    }
}
