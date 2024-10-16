using Bravasoft.Functional.Errors;
using Cysharp.Threading.Tasks;
using System;

namespace Bravasoft.Functional.Async
{
    public sealed class ErrorException : Exception
    {
        public ErrorException(Error error) : base(error.Message)
        {
            Error = error;
        }

        public Error Error { get; }
    }

    public readonly struct AsyncResult<T>
    {
        private readonly AsyncLazy<T> _lazyTask;

        public AsyncResult(UniTask<T> task)
        {
            _lazyTask = UniTask.Lazy(() => task);
        }

        public static AsyncResult<T> Ok(T value)
        {
            Check.AssureNotNull(value, nameof(value));

            return new AsyncResult<T>(UniTask.FromResult(value));
        }

        public static AsyncResult<T> Fail(Exception exception)
        {
            Check.AssureNotNull(exception, nameof(exception));

            return new AsyncResult<T>(UniTask.FromException<T>(exception));
        }

        public static AsyncResult<T> Fail(Error error)
        {
            Check.AssureNotNull(error, nameof(error));

            return new AsyncResult<T>(UniTask.FromException<T>(new ErrorException(error)));
        }

        public AsyncResult<U> Map<U>(Func<T, U> map)
        {
            return Bind(x => AsyncResult<U>.Ok(map(x)));
        }

        public AsyncResult<U> Bind<U>(Func<T, AsyncResult<U>> bind)
        {
            return _lazyTask.Task
                .ContinueWith(t =>
                {
                    try
                    {
                        return bind(t)._lazyTask.Task;
                    }
                    catch (Exception ex)
                    {
                        return UniTask.FromException<U>(ex);
                    }
                })
                .ToResultAsync();
        }

        public async UniTask<U> Match<U>(Func<T, U> onOk, Func<Error, U> onError)
        {
            try
            {
                var t = await _lazyTask;

                return onOk(t);
            }
            catch (Exception ex)
            {
                return onError(ex.ToError());
            }
        }

        public UniTask Iter(Action<T> onOK)
        {
            return _lazyTask.Task.ContinueWith(t =>
            {
                try
                {
                    onOK(t);
                }
                catch (Exception ex)
                {
                }
            });
        }

        public UniTask BiIter(Action<T> onOk, Action<Error> onError)
        {
            return _lazyTask.Task.ContinueWith(t =>
            {
                try
                {
                    onOk(t);
                }
                catch (Exception ex)
                {
                    if (ex is ErrorException errorExcpection)
                        onError(errorExcpection.Error);

                    onError(ex.ToError());
                }
            });
        }
    }

    public static class ResultAsync
    {
        public static AsyncResult<T> Ok<T>(T value) => new AsyncResult<T>(UniTask.FromResult(value));
        //public static ResultAsync<T> Fail<T>(Error error) => new ResultAsync<T>(error);

        public static AsyncResult<T> ToResultAsync<T>(this in UniTask<T> task)
        {
            return new AsyncResult<T>(task);
        }

        public static AsyncResult<T> ToResultAsync<T>(this in Result<T> result)
        {
            return result.Match(AsyncResult<T>.Ok, AsyncResult<T>.Fail);
        }

        public static AsyncResult<T> Unwrap<T>(this AsyncResult<AsyncResult<T>> result)
        {
            return result.Bind(x => x);
        }
    }
}