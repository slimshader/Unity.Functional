namespace Bravasoft.Functional
{
    public static class Int
    {
        public static Option<int> TryParse(string arg) =>
            int.TryParse(arg, out int value) ? value : Option<int>.None;
    }
}
