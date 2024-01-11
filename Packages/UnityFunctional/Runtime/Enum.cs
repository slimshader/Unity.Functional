namespace Bravasoft.Functional
{
    using static Prelude;

    public static class Enum
    {
        public static Option<TEnum> TryParse<TEnum>(string value) where TEnum : struct =>
            System.Enum.TryParse(value, out TEnum result) ? result : none;

        public static Option<TEnum> TryParse<TEnum>(string value, bool ignoreCase) where TEnum : struct =>
            System.Enum.TryParse(value, ignoreCase, out TEnum result) ? result : none;
    }
}
