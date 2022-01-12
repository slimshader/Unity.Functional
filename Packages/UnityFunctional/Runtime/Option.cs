using System;
using System.Collections.Generic;

namespace Bravasoft.Unity.Functional
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

        public Option<T> Filter(Func<T, bool> predicate) => IsSome && predicate(_value) ? Some(_value) : None;

        public (bool IsSome, T Value) AsTuple() => (IsSome, _value);
        public void Deconstruct(out bool isSome, out T value) => (isSome, value) = AsTuple();

        public bool Equals(Option<T> other) =>
            (!IsSome && !other.IsSome) ||
            (IsSome && other.IsSome && EqualityComparer<T>.Default.Equals(_value, other._value));

        public static implicit operator Option<T>(T value) => Some(value);
        public static implicit operator Option<T>(Option.NoneT _) => None;
        public static explicit operator T(Option<T> ot) => ot.IsSome ? ot._value : throw new InvalidOperationException();

        public IEnumerable<T> ToEnumerable()
        {
            if (IsSome) yield return _value;
        }

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
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);

        public readonly struct NoneT { }
        public static readonly NoneT None = new NoneT();
        public static Option<T> ToSome<T>(this T t) => Some(t);

        public static Option<Unit> Condition(Func<bool> cond) =>
            cond() ? Some(Unit.Default) : None;

        public static Option<U> Select<T, U>(this in Option<T> option, Func<T, U> selector) =>
            option.Map(selector);

        public static Option<U> SelectMany<T, T1, U>(this in Option<T> option, Func<T, Option<T1>> selector, Func<T, T1, U> resultSelector) =>
            option.Bind(t => selector(t).Map(t1 => resultSelector(t, t1)));

        public static Option<T> Where<T>(this in Option<T> option, Func<T, bool> predicate) => option.Filter(predicate);
    }
}
