namespace Bravasoft.Functional
{
    public static class Float
    {
        public static Option<float> ParseFloat(string arg) =>
            float.TryParse(arg, out float value) ? Option.Some(value) : Option.None;
    }
}
