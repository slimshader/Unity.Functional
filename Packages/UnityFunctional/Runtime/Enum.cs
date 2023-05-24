namespace Bravasoft.Functional
{
    public static class Enum
    {
        public static Option<TEnum> TryParse<TEnum>(string value) where TEnum : struct =>
            System.Enum.TryParse(value, out TEnum result) ? Option.Some(result) : Option.None;

        public static Option<TEnum> TryParse<TEnum>(string value, bool ignoreCase) where TEnum : struct =>
            System.Enum.TryParse(value, ignoreCase, out TEnum result) ? Option.Some(result) : Option.None;
    }
}
