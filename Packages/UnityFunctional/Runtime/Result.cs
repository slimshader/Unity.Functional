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

        public Result<T> MapError(Func<Error, Error> errorMap) =>
            IsOk ? this : Result.Fail(errorMap(_error));

        public Result<UValue> BiMap<UValue>(Func<T, UValue> map, Func<Error, Error> errorMap) =>
            IsOk ? (Result<UValue>)Result.Ok(map(_value)) : Result.Fail(errorMap(_error));

        public Result<UValue> Bind<UValue>(Func<T, Result<UValue>> bind) =>
            IsOk ? bind(_value) : Result.Fail(_error);

        public U Match<U>(Func<T, U> onOk, Func<Error, U> onError) => IsOk ? onOk(_value) : onError(_error);

        public Option<T> ToOption() => IsOk ? Option<T>.Some(_value) : Option<T>.None;
        public Option<Error> ToErrorOption() => IsOk ? Option<Error>.None : _error;

        public IEnumerable<T> ToEnumerable()
        {
            if (IsOk) yield return _value;
        }

        public bool TryGetValue(out T value)
        {
            value = IsOk ? _value : default;
            return IsOk;
        }

        public bool TryGetError(out Error error)
        {
            error = !IsOk ? _error : default;
            return !IsOk;
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

        public (bool IsOk, T Value, Error Error) AsTuple() => (IsOk, _value, _error);

        public void Deconstruct(out bool isOk, out T value, out Error error) => (isOk, value, error) = AsTuple();

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

        public static Result<Unit> Condition(Func<bool> cond, Error error) =>
            cond() ? (Result<Unit>)Ok(Unit.Default) : Fail(error);

        public static Result<Unit> Condition(Func<bool> cond, Func<Error> onError) =>
            cond() ? (Result<Unit>)Ok(Unit.Default) : Fail(onError());

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

        public static Func<Result<T>> ToResult<T>(this Func<T> f) => () =>
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return Fail(e);
            }
        };
    }
}
