﻿using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
    {
        private readonly T _value;

        public readonly bool IsSome;
        public bool IsNone => !IsSome;

        public static readonly Option<T> None = default;
        public static Option<T> Some(T value) => new Option<T>(true, value);

        public bool TryGetValue(out T value)
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
        public Option<U> MapOptional<U>(Func<T, U> map) => IsSome ? (map(_value) ?? Option<U>.None) : Option.None;
        public Option<U> Bind<U>(Func<T, Option<U>> bind) => IsSome ? bind(_value) : Option.None;
        public Option<T> Filter(Func<T, bool> predicate) => IsSome && predicate(_value) ? Some(_value) : None;
        public Option<U> Cast<U>() => IsSome && _value is U u ? u : Option.None;

        public void Deconstruct(out bool isSome, out T value) => (isSome, value) = (IsSome, _value);

        public bool Equals(Option<T> other) =>
            (!IsSome && !other.IsSome) ||
            (IsSome && other.IsSome && EqualityComparer<T>.Default.Equals(_value, other._value));

        public bool Equals(T value) => IsSome && EqualityComparer<T>.Default.Equals(_value, value);

        public static implicit operator Option<T>(in T value) => Prelude.Optional(value);
        public static implicit operator Option<T>(Option.NoneType _) => None;
        public static explicit operator T(in Option<T> ot) => ot.IsSome ? ot._value : throw new OptionCastEception();

        public static implicit operator bool(in Option<T> result) => result.IsSome;

        public static bool operator ==(in Option<T> option, in Option<T> other) => option.Equals(other);
        public static bool operator !=(in Option<T> option, in Option<T> other) => !option.Equals(other);

        public static bool operator ==(in Option<T> option, in T value) => option.Equals(value);
        public static bool operator !=(in Option<T> option, in T value) => !option.Equals(value);

        public override bool Equals(object obj)
        {
            if (obj is Option<T> other)
                return Equals(other);

            if (obj is T value)
                return Equals(value);

            return false;
        }

        public override int GetHashCode() => (IsSome, _value).GetHashCode();

        public IEnumerable<T> ToEnumerable()
        {
            if (IsSome) yield return _value;
        }

        public OptionEnumerator<T> GetEnumerator() => new OptionEnumerator<T>(this);

        public override string ToString() => IsSome ? $"Some({_value})" : "None";

        private Option(bool isSome, T value)
        {
            IsSome = isSome;
            _value = IsSome ? Check.AssureNotNull(value, nameof(value)) : default;
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);

        public readonly struct NoneType { }
        public static readonly NoneType None = new NoneType();
        public static Option<T> ToSome<T>(this T t) => Some(t);

        public static Option<U> Select<T, U>(this in Option<T> option, Func<T, U> selector) =>
            option.Map(selector);

        public static Option<U> SelectMany<T, T1, U>(this in Option<T> option, Func<T, Option<T1>> selector, Func<T, T1, U> resultSelector) =>
            option.Bind(t => selector(t).Map(t1 => resultSelector(t, t1)));

        public static Option<T> Where<T>(this in Option<T> option, Func<T, bool> predicate) => option.Filter(predicate);

        public static Option<U> Map<T1, T2, U>(this in Option<ValueTuple<T1, T2>> option, Func<T1, T2, U> map) =>
            option.Map(t => map(t.Item1, t.Item2));

        public static Option<U> Map<T1, T2, T3, U>(this in Option<ValueTuple<T1, T2, T3>> option, Func<T1, T2, T3, U> map) =>
            option.Map(t => map(t.Item1, t.Item2, t.Item3));

        public static U Match<T1, T2, U>(this in Option<ValueTuple<T1, T2>> option, Func<T1, T2, U> onSome, Func<U> onNone) =>
            option.Match(t => onSome(t.Item1, t.Item2), onNone);

        public static U Match<T1, T2, T3, U>(this in Option<ValueTuple<T1, T2, T3>> option, Func<T1, T2, T3, U> onSome, Func<U> onNone) =>
            option.Match(t => onSome(t.Item1, t.Item2, t.Item3), onNone);

        public static Unit Iter<T>(this in Option<T> option, Action<T> onSome)
        {
            if (option.TryGetValue(out var v))
                onSome(v);

            return default;
        }
    }
}
