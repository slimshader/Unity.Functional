namespace Bravasoft.Functional
{
    public static class Prelude
    {
        public static Option<T> Optional<T>(T value) =>
            value == null ? Option<T>.None : Option<T>.Some(value);
        
        public static Option<string> OptionalNonEmpty(this string value) =>
            string.IsNullOrEmpty(value) ? Option.None : Option.Some(value);
    }
}
