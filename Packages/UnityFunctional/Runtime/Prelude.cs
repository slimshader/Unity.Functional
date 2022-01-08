namespace Bravasoft.UnityFunctional
{
    public static class Prelude
    {
        public static Option<int> ParseInt(string arg) =>
            int.TryParse(arg, out int value) ? Option.Some(value) : Option.None;
        
        public static Option<float> ParseFloat(string arg) =>
            float.TryParse(arg, out float value) ? Option.Some(value) : Option.None;
    }
}
