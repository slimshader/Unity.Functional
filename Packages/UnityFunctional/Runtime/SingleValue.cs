namespace Bravasoft.Functional
{
    public readonly struct SingleValue<T>
    {
        public readonly T Value;

        public SingleValue(T value)
        {
            Value = value;
        }

        public void Deconstruct(out T value) => value = Value;
    }
}
