using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public readonly struct Result<T> : IEquatable<Result<T>>
    {
        private readonly T _value;
        private readonly Error _error;

        public readonly bool IsOk;

        public static Result<T> Ok(T value) => new Result<T>(true, value, default);
        public static Result<T> Fail(Error error) => new Result<T>(false, default, error);
        public static Result<T> Fail(Exception exception) => new Result<T>(false, default, new ExceptionError(exception));

        public bool IsError<TError>() where TError : Error =>
            _error is Error;

        public bool IsException<TException>() where TException : System.Exception =>
            _error is ExceptionError ee && ee.Exception is TException;

        public T IfError<U>(Func<Error, T> onError) => IsOk ? _value : onError(_error);

        public Result<UValue> Map<UValue>(Func<T, UValue> map) =>
            IsOk ? (Result<UValue>)Result.Ok(map(_value)) : Result.Fail(_error);

        public Result<T> Where(Func<T, bool> predicate) =>
            IsOk && predicate(_value) ? this : FilterError.Default;

        public Result<UValue> BiMap<UValue>(Func<T, UValue> map, Func<Error, Error> errorMap) =>
            IsOk ? (Result<UValue>)Result.Ok(map(_value)) : Result.Fail(errorMap(_error));

        public Result<UValue> Bind<UValue>(Func<T, Result<UValue>> bind) =>
            IsOk ? bind(_value) : Result.Fail(_error);

        public Result<U> TryCast<U>() => IsOk && _value is U u ? u : Result.Fail(new InvalidCastException());

        public U Match<U>(Func<T, U> onOk, Func<Error, U> onError) => IsOk ? onOk(_value) : onError(_error);

        public U MatchError<U, TError>(Func<T, U> onOk, Func<TError, U> onError) where TError : Error =>
            IsOk
            ? onOk(_value)
            : _error is TError error
            ? onError(error)
            : throw new InvalidOperationException("No excpetion match");


        public U MatchException<U, TException>(Func<T, U> onOk, Func<TException, U> onException) where TException : Exception =>
            IsOk
            ? onOk(_value)
            : _error is ExceptionError exe and { Exception: TException tex }
            ? onException(tex)
            : throw new InvalidOperationException("No excpetion match");

        public Option<T> ToOption() => IsOk ? Option<T>.Some(_value) : Option<T>.None;

        public IEnumerable<T> ToEnumerable()
        {
            if (IsOk) yield return _value;
        }

        public bool TryGetValue(out T value)
        {
            value = IsOk ? _value : default;
            return IsOk;
        }

        public Unit Iter(Action<T> onOk)
        {
            if (IsOk)
            {
                onOk(_value);
            }
            return default;
        }

        public Unit BiIter(Action<T> onOk, Action<Error> onError)
        {
            if (IsOk)
            {
                onOk(_value);
            }
            else
            {
                onError(_error);
            }

            return default;
        }

        public void Deconstruct(out bool isOk, out T value, out Error error) => (isOk, value, error) = (IsOk, _value, _error);

        public static explicit operator T(in Result<T> result) =>
            result.IsOk ? result._value : throw new ResultCastException(result._error);

        public static implicit operator bool(in Result<T> result) => result.IsOk;

        public static implicit operator Result<T>(T value) => Ok(value);
        public static implicit operator Result<T>(Error error) => Fail(error);
        public static implicit operator Result<T>(Exception exception) => Fail(exception);

        public static implicit operator Result<T>(in Result.ResultOk<T> ok) => Ok(ok.Value);
        public static implicit operator Result<T>(in Result.ResultFail fail) => Fail(fail.Error);

        public override string ToString() => IsOk ? $"Ok({_value})" : $"Fail({_error})";

        public bool Equals(Result<T> other) => (IsOk, _value, _error).Equals((other.IsOk, other._value, other._error));

        private Result(bool isOk, T value, Error error)
        {
            IsOk = isOk;
            _value = isOk ? Check.AssureNotNull(value, nameof(value)) : default;
            _error = !isOk ? Check.AssureNotNull(error, nameof(error)) : default;
        }
    }

    public static class Result
    {
        public readonly struct ResultOk<TValue>
        {
            public readonly TValue Value;
            public ResultOk(TValue value) => Value = value;
        }

        public readonly struct ResultFail
        {
            public readonly Error Error;
            public ResultFail(Error error) => Error = error;
        }

        public static ResultOk<T> Ok<T>(T value) => new ResultOk<T>(value);

        public static ResultFail Fail() => new ResultFail(Error.Default);
        public static ResultFail Fail(Error error) => new ResultFail(error);
        public static ResultFail Fail(Exception exception) => new ResultFail(new ExceptionError(exception));

        public static Option<T> ToOption<T>(this Result<T> result) =>
            result.Match(Option<T>.Some, _ => Option<T>.None);

        public static Result<T> ToResult<T>(this in Option<T> option, Error error) =>
            option.Match(Result<T>.Ok, () => Result<T>.Fail(error));

        public static Result<T> ToResult<T>(this in Option<T> option, Func<Error> onError) =>
            option.Match(Result<T>.Ok, () => Result<T>.Fail(onError()));

        public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> selector) =>
            result.Map(selector);

        public static Result<TResult> SelectMany<T, U, TResult>(this in Result<T> result,
            Func<T, Result<U>> selector,
            Func<T, U, TResult> resultSelector) =>
            result.Bind(tvalue => selector(tvalue).Bind<TResult>(uvalue => Ok(resultSelector(tvalue, uvalue))));
    }

    public static partial class Prelude
    {
        public static Result<Unit> When(bool condition, Result<Unit> alternative) => condition ? alternative : Result<Unit>.Ok(default);
        public static Result<Unit> Unless(bool condition, Result<Unit> alternative) => When(!condition, alternative);
    }
}
