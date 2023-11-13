namespace Bravasoft.Functional
{
    public static class Float
    {
        public static Option<float> ParseFloat(string arg) =>
            float.TryParse(arg, out float value) ? value : Prelude.None;
    }
}
