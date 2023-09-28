namespace Bravasoft.Functional
{
    public static class NullableExtensions
    {
        public static Option<T> AsOption<T>(this T? value) where T : struct =>
            value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None;
    }
}
