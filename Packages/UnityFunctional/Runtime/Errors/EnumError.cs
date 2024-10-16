namespace Bravasoft.Functional.Errors
{
    public class EnumError<TEnum> : Error where TEnum : struct, System.Enum
    {
        public EnumError(TEnum value)
        {
            Value = value;
        }

        public TEnum Value { get; }

        public override string Message => Value.ToString();
    }

    public static class EnumExtensions
    {
        public static EnumError<TEnum> ToError<TEnum>(this TEnum value) where TEnum : struct, System.Enum =>
            new EnumError<TEnum>(value);
    }
}
