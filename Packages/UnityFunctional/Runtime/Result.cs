using System;
using System.Collections.Generic;

namespace Bravasoft.Unity.Functional
{
    public readonly struct Result<TValue>
    {
        private readonly TValue _value;
        private readonly Error _error;

        public readonly bool IsOk;

        public static Result<TValue> Ok(TValue value) => new Result<TValue>(true, value, default);
        public static Result<TValue> Fail(Error error) => new Result<TValue>(false, default, error);

        public Error Error => IsOk ? throw new InvalidOperationException() : _error;

        public Result<UValue> Map<UValue>(Func<TValue, UValue> map) =>
            IsOk ? (Result<UValue>) Result.Ok(map(_value)) : Result.Fail(_error);

        public Result<TValue> MapError(Func<Error, Error> errorMap) =>
            IsOk ? this : Result.Fail(errorMap(_error));

        public Result<UValue> BiMap<UValue>(Func<TValue, UValue> map, Func<Error, Error> errorMap) =>
            IsOk ? (Result<UValue>) Result.Ok(map(_value)) : Result.Fail(errorMap(_error));

        public Result<UValue> Bind<UValue>(Func<TValue, Result<UValue>> bind) =>
            IsOk ? bind(_value) : Result.Fail(_error);

        public T Match<T>(Func<TValue, T> onOk, Func<Error, T> onError) => IsOk ? onOk(_value) : onError(_error);

        public Option<TValue> ToOption() => IsOk ? Option<TValue>.Some(_value) : Option<TValue>.None;

        public IEnumerable<TValue> ToEnumerable()
        {
            if (IsOk) yield return _value;
        }

        public (bool IsOk, TValue Value, Error Error) AsTuple() => (IsOk, _value, _error);

        public void Deconstruct(out bool isOk, out TValue value, out Error error) => (isOk, value, error) = AsTuple();

        public static explicit operator TValue(in Result<TValue> result) =>
            result.IsOk ? result._value : throw new ResultCastException(result._error);

        public override string ToString() => IsOk ? $"Ok({_value})" : $"Fail({_error})";

        public static implicit operator Result<TValue>(in Result.ResultOk<TValue> ok) => Ok(ok.Value);
        public static implicit operator Result<TValue>(in Result.ResultFail fail) => Fail(fail.Error);

        private Result(bool isOk, TValue value, Error error)
        {
            IsOk = isOk;
            _value = isOk ? (value ?? throw new ArgumentNullException(nameof(value))) : default;
            _error = !isOk ? (error ?? throw new ArgumentNullException(nameof(value))) : default;
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

        public static ResultOk<TValue> Ok<TValue>(TValue value) => new ResultOk<TValue>(value);
        public static ResultFail Fail(Error error) => new ResultFail(error);

        public static Result<Unit> Condition(Func<bool> cond, Error error) =>
            cond() ? (Result<Unit>)Ok(Unit.Default) : Fail(error);

        public static Result<Unit> Tee(Action action)
        {
            action();
            return Ok(Unit.Default);
        }

        public static Option<TValue> ToOption<TValue>(this Result<TValue> result) =>
            result.Match(Option<TValue>.Some, _ => Option<TValue>.None);

        public static Result<TValue> ToResult<TValue>(this in Option<TValue> option, Error error) =>
            option.Match(Result<TValue>.Ok, () => Result<TValue>.Fail(error));

        public static Result<TValue> ToResult<TValue>(this in Option<TValue> option, Func<Error> onError) =>
            option.Match(Result<TValue>.Ok, () => Result<TValue>.Fail(onError()));

        public static Result<TResult> Select<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> selector) =>
            result.Map(selector);

        public static Result<TResult> SelectMany<TValue, UValue, TResult>(this in Result<TValue> result,
            Func<TValue, Result<UValue>> selector,
            Func<TValue, UValue, TResult> resultSelector) =>
            result.Bind(tvalue => selector(tvalue).Bind<TResult>(uvalue => Result.Ok(resultSelector(tvalue, uvalue))));
    }
}
