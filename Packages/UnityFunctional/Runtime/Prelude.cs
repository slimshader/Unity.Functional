using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public static class Prelude
    {
        public static Option<T> Optional<T>(T value) =>
            value == null ? Option<T>.None : Option<T>.Some(value);
        public static Option<T> AsOption<T>(this T value) => Optional(value);
        public static Option<U> BindOptional<T, U>(this in Option<T> option, Func<T, U> binderOptional) =>
            option.Bind(x => Optional(binderOptional(x)));
        public static Option<string> OptionalNonEmpty(this string value) =>
            string.IsNullOrEmpty(value) ? Option.None : Option.Some(value);

        public static Option<string> TryNonEmpty(this string value) => OptionalNonEmpty(value);
        public static Option<IList<T>> TryNonEmpty<T>(this IList<T> value) => CollectionsGeneric.OptionalNonEmpty(value);
        public static Option<List<T>> TryNonEmpty<T>(this List<T> value) => List.OptionalNonEmpty(value);

        public static Result<T> TryRun<T>(Func<T> f)
        {
            try
            {
                return Result.Ok(f());
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }
        }

        public static Option<T> TryRunOptional<T>(Func<T> f)
        {
            try
            {
                return Option.Some(f());
            }
            catch (Exception)
            {
                return Option.None;
            }
        }

    }
}
