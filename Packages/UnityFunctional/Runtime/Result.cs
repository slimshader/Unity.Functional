using System;

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
            public static new readonly PredicateFail Default = new PredicateFail();
            public PredicateFail() : base("Filter error") { }
        }
    }

    public readonly struct Result<T> : IEquatable<Result<T>>
    {
        private readonly (bool IsOk, T Value, Error Error) _data;

        public bool IsOk => _data.IsOk;

        public static Result<T> Ok(T value) => new Result<T>(value);
        public static Result<T> Fail(Error error) => new Result<T>(error);
        public static Result<T> Fail(Exception exception) => new Result<T>(new ExceptionError(exception));

        public bool IsError<TError>() where TError : Error => _data.Error is TError;

        public bool IsException<TException>() where TException : System.Exception =>
            _data.Error is ExceptionError ee && ee.Exception is TException;

        public T IfError<U>(Func<Error, T> onError) =>
            IsOk
            ? _data.Value
            : onError(_data.Error);

        public T IfFailDefault() =>
            IsOk
            ? _data.Value
            : default;

        public Result<UValue> Map<UValue>(Func<T, UValue> map) =>
            IsOk
            ? (Result<UValue>)Result.Ok(map(_data.Value))
            : Result.Fail(_data.Error);

        public Result<T> Where(Func<T, bool> predicate) =>
            IsOk && predicate(_data.Value)
            ? this
            : Errors.PredicateFail.Default;

        public Result<UValue> BiMap<UValue>(Func<T, UValue> map, Func<Error, Error> errorMap) =>
            IsOk ? (Result<UValue>)Result.Ok(map(_data.Value)) : Result.Fail(_data.Error);

        public Result<UValue> Bind<UValue>(Func<T, Result<UValue>> bind) =>
            IsOk
            ? bind(_data.Value)
            : Result.Fail(_data.Error);

        public Result<U> TryCast<U>() =>
            IsOk && _data.Value is U u
            ? u
            : Result.Fail(new Errors.InvalidCast());

        public U Match<U>(Func<T, U> onOk, Func<Error, U> onError) =>
            IsOk
            ? onOk(_data.Value)
            : onError(_data.Error);

        public U MatchError<U, TError>(Func<T, U> onOk, Func<TError, U> onError) where TError : Error =>
            IsOk
            ? onOk(_data.Value)
            : _data.Error is TError error
            ? onError(error)
            : throw new InvalidOperationException("No excpetion match");


        public U MatchException<U, TException>(Func<T, U> onOk, Func<TException, U> onException) where TException : Exception =>
            IsOk
            ? onOk(_data.Value)
            : _data.Error is ExceptionError and { Exception: TException tex }
            ? onException(tex)
            : throw new InvalidOperationException("No excpetion match");

        public Option<T> ToOption() => IsOk ? Option<T>.Some(_data.Value) : Option<T>.None;

        public bool TryGetValue(out T value)
        {
            value = IsOk ? _data.Value : default;
            return IsOk;
        }

        public bool TryGetError(out Error error)
        {
            error = !IsOk ? _data.Error : default;
            return !IsOk;
        }

        public Unit Iter(Action<T> onOk)
        {
            if (IsOk)
            {
                onOk(_data.Value);
            }
            return default;
        }

        public Unit BiIter(Action<T> onOk, Action<Error> onError)
        {
            if (IsOk)
            {
                onOk(_data.Value);
            }
            else
            {
                onError(_data.Error);
            }

            return default;
        }

        public void Deconstruct(out bool isOk, out T value, out Error error) => (isOk, value, error) = _data;

        public static explicit operator T(in Result<T> result) =>
            result.IsOk ? result._data.Value : throw new ResultCastException(result._data.Error);

        public static implicit operator bool(in Result<T> result) => result.IsOk;

        public static implicit operator Result<T>(T value) => Ok(value);
        public static implicit operator Result<T>(Error error) => Fail(error);
        public static implicit operator Result<T>(Exception exception) => Fail(exception);

        public static implicit operator Result<T>(in Result.ResultOk<T> ok) => Ok(ok.Value);
        public static implicit operator Result<T>(in Result.ResultFail fail) => Fail(fail.Error);

        public override string ToString() => IsOk ? $"Ok({_data.Value})" : $"Fail({_data.Error})";

        public bool Equals(Result<T> other) => _data.Equals(other._data);

        private Result(T value) => _data = (true, Check.AssureNotNull(value, nameof(value)), default);
        private Result(Error error) => _data = (false, default, Check.AssureNotNull(error, nameof(error)));
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
