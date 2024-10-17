using Cysharp.Threading.Tasks;
using System;

namespace Bravasoft.Functional.Async
{
    public sealed class ResultErrorException : Exception
    {
        public ResultErrorException(Error error) : base(error.Message)
        {
            Error = error;
        }
        public Error Error { get; }
    }

    public static class UniTaskResultExtensions
    {
        public static UniTask<U> Bind<T, U>(this UniTask<T> task, Func<T, UniTask<U>> bind) =>
            task.ContinueWith(bind);

        public static UniTask<U> Map<T, U>(this UniTask<T> task, Func<T, U> map) =>
            task.ContinueWith(map);


        public static async UniTask<Result<U>> Bind<T, U>(this UniTask<Result<T>> task, Func<T, UniTask<Result<U>>> func)
        {
            var result = await task;

            var (isOk, value, error) = result;

            if (isOk)
            {
                return await func(value);
            }

            return error;
        }

        public static async UniTask<Result<U>> MapT<T, U>(this UniTask<Result<T>> task, Func<T, U> func)
        {
            var result = await task;
            var (isOk, value, error) = result;
            if (isOk)
            {
                return func(value);
            }
            return error;
        }

        public static UniTask Iter<T>(this UniTask<T> task, Action<T> onResult) =>
            task.ContinueWith(onResult);

        public static UniTask Iter<T>(this UniTask<Result<T>> task, Action<T> onResult) =>
            task.ContinueWith(result =>
            {
                var (isOk, value, error) = result;
                if (isOk)
                {
                    onResult(value);
                }
            });


        public static UniTask Iter<T>(this UniTask<Result<T>> task, Func<T, UniTask> onOkResult) =>
            task.ContinueWith(async result =>
            {
                var (isOk, value, error) = result;
                if (isOk)
                {
                    await onOkResult(value);
                }
            });

        public static async void BiIter<T>(this UniTask<T> task, Action<T> onResult, Action<Exception> onException)
        {
            try
            {
                var result = await task;
                onResult(result);
            }
            catch (Exception ex)
            {
                onException(ex);
            }
        }

        public static async UniTask<T> IfException<T>(this UniTask<T> task, T value)
        {
            try
            {
                return await task;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public static async UniTask<T> IfException<T>(this UniTask<T> task, Func<T> valueFunc)
        {
            try
            {
                return await task;
            }
            catch (Exception)
            {
                return valueFunc();
            }
        }

        public static UniTask<T> Flatten<T>(this UniTask<Result<T>> resultTask) =>
            resultTask.Map(result =>
            {
                var (isOk, value, error) = result;
                if (isOk)
                {
                    return value;
                }
                throw new ResultErrorException(error);
            });

        public static UniTask<U> Select<T, U>(this UniTask<T> task, Func<T, U> map) =>
            task.Map(map);

        public static UniTask<U> SelectMany<T, T1, U>(this UniTask<T> task,
            Func<T, UniTask<T1>> selector,
            Func<T, T1, UniTask<U>> bind) =>
            task.Bind(a => selector(a).Bind(b => bind(a, b)));

        public static UniTask<Result<U>> Select<T, U>(this UniTask<Result<T>> task, Func<T, U> map) =>
            task.MapT(map);

        public static UniTask<Result<U>> SelectMany<T, T1, U>(this UniTask<Result<T>> task,
            Func<T, UniTask<Result<T1>>> selector,
            Func<T, T1, UniTask<Result<U>>> bind) =>
            task.Bind(a => selector(a).Bind(b => bind(a, b)));
    }
}
