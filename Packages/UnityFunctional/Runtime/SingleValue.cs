using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public readonly struct SingleValue<T> : IEquatable<SingleValue<T>>
    {
        public readonly T Value;

        public SingleValue(T value)
        {
            Value = value;
        }

        public void Deconstruct(out T value) => value = Value;

        public bool Equals(SingleValue<T> other) => EqualityComparer<T>.Default.Equals(Value, other.Value);

        public ValueTuple<T, T> Append(T value) => (Value, value);
    }
}
