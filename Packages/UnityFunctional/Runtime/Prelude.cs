namespace Bravasoft.Functional
{
    public static partial class Prelude
    {
        public static Core.NoneType none => Core.None;
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);
        public static Option<T> None<T>() => Option<T>.None;

        public static Result<T> Ok<T>(T value) => Result.Ok(value);
        public static Result<T> Fail<T>(string message) => Result.Fail(new Error(message));

        public static Option<T> Optional<T>(T value) =>
            value == null ? Option<T>.None : Option<T>.Some(value);

        public static Option<T> AsOption<T>(this T value) => Optional(value);

        public static Option<string> TryNotNullOrEmpty(this string str) =>
            string.IsNullOrEmpty(str) ? none : Some(str);

        public static Option<string> TryNotNullOrWhiteSpace(this string str) =>
            string.IsNullOrWhiteSpace(str) ? none : Some(str);

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
