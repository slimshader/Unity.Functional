using System;

namespace UnityFunctional
{
    public readonly struct Result<TValue, TError>
    {
        private readonly TValue _value;
        private readonly TError _error;
        private readonly bool _isOk;

        public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(true, value, default);
        public static Result<TValue, TError> Fail(TError error) => new Result<TValue, TError>(false, default, error);

        public bool IsOk => _isOk;

        public TError Error => _isOk ? throw new InvalidOperationException() : _error;

        public Result<UValue, TError> Map<UValue>(Func<TValue, UValue> map) =>
            _isOk ? (Result<UValue, TError>) Result.Ok(map(_value)) : Result.Fail(_error);

        public Result<UValue, TError> Bind<UValue>(Func<TValue, Result<UValue, TError>> bind) =>
            _isOk ? bind(_value) : Result.Fail(_error);

        public T Match<T>(Func<TValue, T> ok, Func<TError, T> error) => _isOk ? ok(_value) : error(_error);

        public Option<TValue> ToOption() => _isOk ? Option<TValue>.Some(_value) : Option<TValue>.None;

        public static explicit operator TValue(in Result<TValue, TError> result) =>
            result.IsOk ? result._value : throw new ResultFailException<TError>(result._error);

        public override string ToString() => _isOk ? $"Ok({_value})" : $"Fail({_error})";

        public static implicit operator Result<TValue, TError>(in Result.ResultOk<TValue> ok) => Ok(ok.Value);
        public static implicit operator Result<TValue, TError>(in Result.ResultFail<TError> fail) => Fail(fail.Error);

        private Result(bool isOk, TValue value, TError error)
        {
            _isOk = isOk;
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

        public readonly struct ResultFail<TError>
        {
            public readonly TError Error;
            public ResultFail(TError error) => Error = error;
        }

        public static ResultOk<TValue> Ok<TValue>(TValue value) => new ResultOk<TValue>(value);
        public static ResultFail<TError> Fail<TError>(TError error) => new ResultFail<TError>(error);

        public static Option<TValue> ToOption<TValue, TError>(this Result<TValue, TError> result) =>
            result.Match(Option<TValue>.Some, _ => Option<TValue>.None);

        public static Result<TValue, TError> ToResult<TValue, TError>(this in Option<TValue> option, Func<TError> onError) =>
            option.Match(Result<TValue, TError>.Ok, () => Result<TValue, TError>.Fail(onError()));
    }

    public static class ResultLinqExtensions
    {
        public static Result<TResult, TError> Select<TValue, TResult, TError>(this Result<TValue, TError> result, Func<TValue, TResult> selector) =>
            result.Map(selector);

        public static Result<TResult, TError> SelectMany<TValue, UValue, TResult, TError>(this in Result<TValue, TError> result,
            Func<TValue, Result<UValue, TError>> selector,
            Func<TValue, UValue, TResult> resultSelector) =>
            result.Bind(tvalue => selector(tvalue).Bind<TResult>(uvalue => Result.Ok(resultSelector(tvalue, uvalue))));
    }
}
