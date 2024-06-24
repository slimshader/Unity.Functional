using System;

namespace Bravasoft.Functional
{
    public sealed class Try<T>
    {
        private readonly Func<T> _func;

        public Try(Func<T> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func), "Func can not be null");
        }

        public Try<U> Map<U>(Func<T, U> map) => new Try<U>(() => map(_func()));

        public Try<U> Bind<U>(Func<T, Try<U>> bind) => new Try<U>(() => bind(_func())._func());

        public U Match<U>(Func<T, U> onValue, Func<Exception, U> onException)
        {
            try
            {
                return onValue(_func());
            }
            catch (Exception e)
            {
                return onException(e);
            }
        }

        public T IfException(T fallback) => Match(x => x, e => fallback);

        private T Run()
        {
            try
            {
                return _func();
            }
            catch (Exception e)
            {
                throw new TryRunException(e);
            }
        }
    }

    public sealed class TryRunException : Exception
    {
        public TryRunException(Exception innerException) : base("An error occurred in Try", innerException)
        {
        }
    }

    public static class Try
    {
        public static Try<T> Of<T>(Func<T> func) => new Try<T>(func);
        public static Try<T> ToTry<T>(this Func<T> func) => Of(func);

        // LINQ support
        public static Try<U> Select<T, U>(this Try<T> @try, Func<T, U> map) =>
            @try.Map(map);

        public static Try<V> SelectMany<T, U, V>(this Try<T> @try, Func<T, Try<U>> bind, Func<T, U, V> project) =>
            @try.Bind(x => bind(x).Map(y => project(x, y)));

        // Result and Option conversions
        public static Result<T> ToResult<T>(this Try<T> @try) => @try.Match(
            onValue: x => Result.Ok(x),
            onException: e => Result<T>.Fail(new ExceptionError(e)));

        public static Option<T> ToOption<T>(this Try<T> @try) => @try.Match(
            onValue: x => Option.Some(x),
            onException: e => Option.None<T>());
    }
}
