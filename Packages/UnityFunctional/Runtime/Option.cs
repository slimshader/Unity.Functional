using System;
using System.Collections.Generic;

namespace Bravasoft.UnityFunctional
{
    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T _value;

        public readonly bool IsSome;

        public static readonly Option<T> None = default;
        public static Option<T> Some(T value) => new Option<T>(true, value);

        public bool TryGetSome(out T value)
        {
            if (IsSome)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        public T IfNone(T v) => IsSome ? _value : v;
        public T IfNone(Func<T> fv) => IsSome ? _value : fv();

        public U Match<U>(Func<T, U> onSome, Func<U> onNone) =>
            IsSome ? onSome(_value) : onNone();

        public Option<U> Map<U>(Func<T, U> map) => IsSome ? Option.Some(map(_value)) : Option.None;

        public Option<U> Bind<U>(Func<T, Option<U>> bind) => IsSome ? bind(_value) : Option.None;

        public Option<T> Where(Func<T, bool> predicate) => IsSome && predicate(_value) ? Some(_value) : None;

        public (bool IsSome, T Value) AsTuple() => (IsSome, _value);
        public void Deconstruct(out bool isSome, out T value) => (isSome, value) = AsTuple();

        public bool Equals(Option<T> other) =>
            (!IsSome && !other.IsSome) ||
            (IsSome && other.IsSome && EqualityComparer<T>.Default.Equals(_value, other._value));

        public static implicit operator Option<T>(T value) => Option.Of(value);
        public static implicit operator Option<T>(Option.NoneT _) => None;
        public static explicit operator T(Option<T> ot) => ot.IsSome ? ot._value : throw new InvalidOperationException();

        public override string ToString() => IsSome ? $"Some({_value})" : "None";

        private Option(bool isSome, T value)
        {
            IsSome = isSome;

            if (IsSome)
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                _value = value;
            }
            else
            {
                _value = default;
            }
        }
    }

    public static class Option
    {
        public static Option<T> Of<T>(T value) =>        
            value == null ? Option<T>.None : Option<T>.Some(value);

        public static Option<T> Some<T>(T value) => Option<T>.Some(value);

        public readonly struct NoneT { }
        public static readonly NoneT None = new NoneT();

        public static Option<T> ToOption<T>(this T t) => Of(t);
        public static Option<T> ToSome<T>(this T t) => Some(t);

        public static Option<TValue> Condition<TValue>(Func<bool> cond, TValue value) =>
            cond() ? Option.Some(value) : Option.None;
        public static Option<TValue> Condition<TValue>(Func<bool> cond, Func<TValue> onValue) =>
            cond() ? Option.Some(onValue()) : Option.None;

    }
}
