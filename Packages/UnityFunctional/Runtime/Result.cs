using System;
using static Bravasoft.Functional.Prelude;

namespace Bravasoft.Functional
{
    namespace Errors
    {
        sealed class InvalidCast : Error
        {
            public InvalidCast() : base("Invalid cast") { }
        }

        public sealed class PredicateFail : Error
        {
            public PredicateFail(Error inner) : base("Filter failed")
            {
                Inner = inner;
            }

            public Error Inner { get; }
        }

        public sealed class Uninitialized : Error
        {
            public static new readonly Uninitialized Default = new Uninitialized();
            public Uninitialized() : base("Uninitialized") { }
        }
    }

    public readonly struct Result<T> : IEquatable<Result<T>>
    {
        private readonly (Option<T> MaybeValue, Error Error) _data;

        public bool IsOk => _data.MaybeValue.IsSome;
        public bool IsFail => !IsOk;

        public static Result<T> Ok(T value) => new Result<T>(value);
        public static Result<T> Fail(Error error) => new Result<T>(error);
        public static Result<T> Fail(Exception exception) => new Result<T>(new ExceptionError(exception));

        public bool IsError<TError>() where TError : Error => _data.Error is TError;
        public bool IsException<TException>() where TException : System.Exception =>
            _data.Error is ExceptionError ee && ee.Exception is TException;

        public T IfFail(T fallback)
        {
            if (Check.IsNull(fallback))
                throw new ArgumentNullException(nameof(fallback));

            return _data.MaybeValue.IfNone(fallback);
        }

        public Result<UValue> Map<UValue>(Func<T, UValue> map) =>
            _data.MaybeValue
            .Map(map)
            .ToResult(_data.Error);

        public Result<T> Where(Func<T, bool> predicate) =>
            _data.MaybeValue
            .Where(predicate)
            .ToResult(new Errors.PredicateFail(_data.Error));

        public Result<UValue> Bind<UValue>(Func<T, Result<UValue>> bind)
        {
            if (_data.MaybeValue.TryGetValue(out var value))
            {
                return bind(value);
            }

            return Result<UValue>.Fail(_data.Error);
        }

        public Result<U> TryCast<U>() =>
            _data.MaybeValue.TryCast<U>().ToResult(new Errors.InvalidCast());

        public U Match<U>(Func<T, U> onOk, Func<Error, U> onError)
        {
            if (_data.MaybeValue.TryGetValue(out var value))
            {
                return onOk(value);
            }

            return onError(_data.Error);
        }

        public Option<T> ToOption() => _data.MaybeValue;
        public Option<Error> ToErrorOption() => TryGetError(out var error) ? Some(error) : none;
        public bool TryGetValue(out T value) => _data.MaybeValue.TryGetValue(out value);

        public bool TryGetError(out Error error)
        {
            error = IsFail ? _data.Error is null ? Errors.Uninitialized.Default : null : null;
            return IsFail;
        }

        public bool TryGetError<TError>(out TError error) where TError : Error
        {
            error =
                IsFail
                ? _data.Error is TError terror
                ? terror
                : null
                : null;

            return error is object;
        }

        public bool TryGetException(out Exception exception)
        {
            exception = IsFail
                ? _data.Error is ExceptionError ee
                ? ee.Exception
                : null
                : null;

            return exception is object;
        }

        public Unit Iter(Action<T> onOk) => _data.MaybeValue.Iter(onOk);

        public Unit ErrorIter(Action<Error> onError)
        {
            if (!IsOk)
            {
                onError(_data.Error);
            }
            return default;
        }

        public void BiIter(Action<T> onOk, Action<Error> onError)
        {
            _data.MaybeValue.Iter(onOk);
            
            if (!IsOk)
                onError(_data.Error);
        }

        public void Deconstruct(out bool isOk, out T value, out Error error)
        {
            (isOk, value) = _data.MaybeValue;           
            error = _data.Error;
        }

        public static explicit operator T(in Result<T> result) =>
            result.IsOk ? (T) result._data.MaybeValue : throw new ResultCastException(result._data.Error);

        public static implicit operator bool(in Result<T> result) => result.IsOk;

        public static implicit operator Result<T>(T value) => Ok(value);
        public static implicit operator Result<T>(Error error) => Fail(error);
        public static implicit operator Result<T>(Exception exception) => Fail(exception);

        public static implicit operator Result<T>(in Result.ResultOk<T> ok) => Ok(ok.Value);
        public static implicit operator Result<T>(in Result.ResultFail fail) => Fail(fail.Error);

        public override string ToString()
        {
            if (_data.MaybeValue.TryGetValue(out var value))
            {
                return $"Ok({value})";
            }

            return $"Fail({_data.Error})";
        }

        public bool Equals(Result<T> other) => _data.Equals(other._data);

        private Result(T value) => _data = (Check.AssureNotNull(value, nameof(value)), default);
        private Result(Error error) => _data = (default, Check.AssureNotNull(error, nameof(error)));
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
