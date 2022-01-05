using System;
using System.Collections.Generic;

namespace UnityFunctional
{
    public static class Option
    {
        public static Option<T> Of<T>(T t)
        {
            if (t == null)
                return Option<T>.None;

            return Option<T>.Some(t);
        }

        public static Option<T> Some<T>(T value) => new Option<T>(value);
        public readonly struct NoneT { }

        public static readonly NoneT None = new NoneT();

        public static Option<int> TryParseInt(string arg) =>
            int.TryParse(arg, out int value) ? Some(value) : None;

        public static Option<T> ToOption<T>(this T t) => Of(t);

        public static Option<T> ToSome<T>(this T t) => Some(t);
    }

    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T _value;

        public static readonly Option<T> None = new Option<T>();
        public static Option<T> Some(T value) => new Option<T>(value);

        public Option(T value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            IsSome = true;
            _value = value;
        }

        public readonly bool IsSome;

        public U Match<U>(Func<T, U> some, Func<U> none) =>
            IsSome ? some(_value) : none();

        public U MatchSome<U>(Func<T, U> p) =>
            IsSome ? p(_value) : throw new InvalidOperationException();

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

        public Option<U> Map<U>(Func<T, U> map) => IsSome ? new Option<U>(map(_value)) : Option<U>.None;

        public Option<U> Bind<U>(Func<T, Option<U>> bind) => IsSome ? bind(_value) : Option<U>.None;

        public Option<T> Where(Func<T, bool> predicate) => IsSome && predicate(_value) ? Some(_value) : None;

        public bool Equals(Option<T> other) =>
            (!IsSome && !other.IsSome) ||
            (IsSome && other.IsSome && EqualityComparer<T>.Default.Equals(_value, other._value));

        public static implicit operator Option<T>(T value) => Option.Of(value);
        public static implicit operator Option<T>(Option.NoneT _) => None;
        public static explicit operator T(Option<T> ot) => ot.IsSome ? ot._value : throw new InvalidOperationException();

        public override string ToString() => IsSome ? $"Some({_value})" : "None";
    }
}
