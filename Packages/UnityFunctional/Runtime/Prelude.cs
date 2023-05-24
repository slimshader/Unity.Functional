using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public static class Prelude
    {
        public static Option<T> Optional<T>(T value) =>
            value == null ? Option<T>.None : Option<T>.Some(value);

        public static Option<T> AsOption<T>(this T value) => Optional(value);
        
        public static Option<string> OptionalNonEmpty(this string value) =>
            string.IsNullOrEmpty(value) ? Option.None : Option.Some(value);

        public static Option<string> TryNonEmpty(this string value) => OptionalNonEmpty(value);
        public static Option<IList<T>> TryNonEmpty<T>(this IList<T> value) => CollectionsGeneric.OptionalNonEmpty(value);
        public static Option<List<T>> TryNonEmpty<T>(this List<T> value) => CollectionsGeneric.OptionalNonEmpty(value);

    }
}
