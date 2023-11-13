namespace Bravasoft.Functional
{
    public static class String
    {
        public static Option<string> TryNonEmpty(this string value) =>
            string.IsNullOrEmpty(value) ? Prelude.None : value;
    }
}
