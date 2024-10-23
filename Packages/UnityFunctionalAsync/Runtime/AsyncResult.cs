using Bravasoft.Functional.Errors;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bravasoft.Functional.Async
{
    public sealed class NoneOptionError : Error
    {
        public static readonly NoneOptionError Default = new NoneOptionError();

        private NoneOptionError() : base("Option is None")
        {
        }
    }

    public sealed class ErrorException : Exception
    {
        public ErrorException(Error error) : base(error.Message)
        {
            Error = error;
        }

        public Error Error { get; }
    }

    public readonly struct AsyncResult<T> : IAsyncEnumerable<T>
    {
        private readonly AsyncLazy<T> _lazyTask;

        public UniTask<T> Task => _lazyTask.Task;

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
                .ToAsyncResult();
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
            if (_lazyTask is null)
                throw new InvalidOperationException("Result is not initialized");

            return _lazyTask.Task.ContinueWith(t =>
            {
                try
                {
                    onOK(t);
                }
                catch (Exception)
                {
                }
            });
        }

        public UniTask IterAsync(Func<T, UniTask> onOK)
        {
            if (_lazyTask is null)
                throw new InvalidOperationException("Result is not initialized");

            return _lazyTask.Task.ContinueWith(async t =>
            {
                try
                {
                    await onOK(t);
                }
                catch (Exception)
                {
                }
            });
        }

        public UniTask Iter(Func<T, UniTask> onOK)
        {
            return _lazyTask.Task.ContinueWith(t =>
            {
                try
                {
                    return onOK(t);
                }
                catch (Exception)
                {
                }

                return UniTask.CompletedTask;
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

        public async UniTask<T> IfError(Func<Error, T> onError)
        {
            try
            {
                return await _lazyTask.Task;
            }
            catch (Exception ex)
            {
                if (ex is ErrorException errorExcpection)
                    return onError(errorExcpection.Error);
                return onError(ex.ToError());
            }
        }

        class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly AsyncResult<T> _result;
            private bool _wasRead;
            public AsyncEnumerator(AsyncResult<T> result)
            {
                _result = result;
                _wasRead = false;
            }
            public T Current { get; private set; }
            public ValueTask DisposeAsync() => new ValueTask();
            public async ValueTask<bool> MoveNextAsync()
            {
                if (_wasRead)
                {
                    return false;
                }

                try
                {
                    Current = await _result._lazyTask.Task;
                    _wasRead = true;
                    return true;
                }
                catch (Exception)
                {
                    _wasRead = true;
                    return false;
                }
            }
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) =>
            new AsyncEnumerator(this);
    }

    public static class AsyncResult
    {
        public static AsyncResult<T> Ok<T>(T value) => new AsyncResult<T>(UniTask.FromResult(value));

        public static AsyncResult<T> ToAsyncResult<T>(this in UniTask<T> task)
        {
            return new AsyncResult<T>(task);
        }

        public static AsyncResult<T> ToAsyncResult<T>(this in Result<T> result)
        {
            return result.Match(AsyncResult<T>.Ok, AsyncResult<T>.Fail);
        }

        public static AsyncResult<T> ToAsyncResult<T>(this in UniTask<Result<T>> resultTask)
        {            
            return resultTask.ContinueWith(result =>
            {
                var (isOk, value, error) = result;
                return isOk ? UniTask.FromResult(value) : UniTask.FromException<T>(new ErrorException(error));
            }).ToAsyncResult();
        }

        public static AsyncResult<T> ToAsyncResult<T>(this in Option<T> option) =>
            option.Match(Ok, () => AsyncResult<T>.Fail(NoneOptionError.Default));


        public static AsyncResult<T> Unwrap<T>(this AsyncResult<AsyncResult<T>> result)
        {
            return result.Bind(x => x);
        }
    }
}