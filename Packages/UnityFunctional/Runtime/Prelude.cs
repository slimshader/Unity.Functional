namespace Bravasoft.Unity.Functional
{
    public static class Prelude
    {
        public static SingleValue<T> ToTuple<T>(this T value) => new SingleValue<T>(value);
        public static Option<T> Optional<T>(T value) =>
            value == null ? Option<T>.None : Option<T>.Some(value);
        public static Option<int> ParseInt(string arg) =>
            int.TryParse(arg, out int value) ? Option.Some(value) : Option.None;
        
        public static Option<float> ParseFloat(string arg) =>
            float.TryParse(arg, out float value) ? Option.Some(value) : Option.None;
    }
}
