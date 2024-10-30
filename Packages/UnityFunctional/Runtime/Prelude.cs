using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public static partial class Prelude
    {
        public static Core.NoneType none => Core.None;
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);
        public static Option<T> None<T>() => Option<T>.None;

        public static Result<T> Ok<T>(T value) => Result.Ok(value);
        public static Result<T> Fail<T>(string message) => Result.Fail(new Error(message));

        public static Try<T> Try<T>(Func<T> func) => new Try<T>(func);

        public static Option<T> Optional<T>(T value) =>
            Check.IsNull(value) ? Option<T>.None : Option<T>.Some(value);

        public static Option<T> AsOption<T>(this T value) => Optional(value);

        public static Option<string> NotEmpty(this in Option<string> option) =>
            option.Where(str => !string.IsNullOrEmpty(str));

        public static Option<string> NotWhiteSpace(this in Option<string> option) =>
            option.Where(str => !string.IsNullOrWhiteSpace(str));

        public static Option<string> AsNotEmptyOption(this string str) =>
            Optional(str).NotEmpty();

        public static Option<string> AsNotWhiteSpaceOption(this string str) =>
            Optional(str).NotWhiteSpace();

        public static Option<List<T>> NotEmpty<T>(this in Option<List<T>> list) =>
            list.Where(lst => lst.Count > 0);

        public static Option<IReadOnlyList<T>> NotEmpty<T>(this in Option<IReadOnlyList<T>> list) =>
            list.Where(lst => lst.Count > 0);

        public static Option<List<T>> AsNotEmptyOption<T>(this List<T> list) =>
            Optional(list).NotEmpty();

        public static Option<IReadOnlyList<T>> AsNotEmptyOption<T>(this IReadOnlyList<T> list) =>
            Optional(list).NotEmpty();

        public static Option<int> TryParseInt(string arg) =>
            int.TryParse(arg, out int value) ? value : none;

        public static Option<float> TryParseFloat(string arg) =>
            float.TryParse(arg, out float value) ? value : none;

        public static Option<double> TryParseDouble(string arg) =>
            double.TryParse(arg, out double value) ? value : none;

        public static Option<bool> TryParseBool(string arg) =>
            bool.TryParse(arg, out bool value) ? value : none;
    }
}
