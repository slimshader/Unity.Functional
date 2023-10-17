using Cysharp.Threading.Tasks;
using System;

namespace Bravasoft.Functional
{
    public static class Async
    {
        public static async UniTask<U> Match<T, U>(this UniTask<T> task, Func<T, U> onValue, Func<Exception, U> onError)
        {
            try
            {
                return onValue(await task);
            }
            catch (Exception ex)
            {
                return onError(ex);
            }
        }
        public static async UniTask<U> Map<T, U>(this UniTask<T> task, Func<T, U> map) => map(await task);

        public static async UniTask<U> Bind<T, U>(this UniTask<T> task, Func<T, UniTask<U>> bind) => await bind(await task);

        public static async UniTask<Result<U>> Map<T, U>(this UniTask<Result<T>> asyncResult, Func<T, U> map)
        {
            try
            {
                var t = await asyncResult;
                return t.Map(map);
            }
            catch (Exception ex)
            {
                return Result<U>.Fail(ex);
            }
        }

        public static async UniTask<Result<U>> Bind<T, U>(this UniTask<Result<T>> asyncResult, Func<T, UniTask<Result<U>>> bind)
        {
            try
            {
                var (isOk, value, error) = await asyncResult;

                if (isOk)
                {
                    return await bind(value);
                }

                return Result<U>.Fail(error);
            }
            catch (Exception ex)
            {
                return Result<U>.Fail(ex);
            }
        }

        public static Func<UniTask<Result<T>>> ToAsyncResult<T>(this Func<UniTask<T>> factory) =>
            () => factory().ToAsyncResult();

        public static async UniTask<T> IfFailed<T>(this UniTask<T> task, Func<UniTask<T>> onFailed)
        {
            try
            {
                return await task;
            }
            catch (Exception _) { }

            return await onFailed();
        }

        public static async UniTask<Result<T>> IfFailed<T>(this UniTask<Result<T>> task, Func<UniTask<Result<T>>> fallback)
        {
            try
            {
                var result = await task;

                if (result.IsOk)
                    return result;

                return await fallback();
            }
            catch (Exception _) { }

            return await fallback();
        }

        public static UniTask<T> Retry<T>(this Func<UniTask<T>> factory, int retries, int delay) =>
            retries <= 0
            ? factory()
            : factory().IfFailed(async () =>
            {
                await UniTask.Delay(delay);
                return await Retry(factory, retries - 1, (int)(delay * 1.5f));
            });

        public static UniTask<Result<T>> Retry<T>(this Func<UniTask<Result<T>>> factory, int retries, int delay) =>
            retries <= 0
            ? factory()
            : factory().IfFailed(async () =>
            {
                await UniTask.Delay(delay);
                return await Retry(factory, retries - 1, (int)(delay * 1.5f));
            });


        public static async UniTask<Result<T>> ToAsyncResult<T>(this UniTask<T> task)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return Result<T>.Fail(ex);
            }
        }

        // LINQ Support

        public static UniTask<U> Select<T, U>(this UniTask<T> task, Func<T, U> selector) =>
            task.Map(selector);

        public static UniTask<Result<U>> Select<T, U>(this UniTask<Result<T>> task, Func<T, U> selector) =>
            task.Map(selector);

        public static UniTask<U> SelectMany<T, T1, U>(this UniTask<T> task, Func<T, UniTask<T1>> selector, Func<T, T1, U> resultSelector) =>
            task.Bind(t => selector(t).Map(t1 => resultSelector(t, t1)));

        public static UniTask<Result<U>> SelectMany<T, T1, U>(this UniTask<Result<T>> task, Func<T, UniTask<Result<T1>>> selector, Func<T, T1, U> resultSelector) =>
            task.Bind(t => selector(t).Map(t1 => resultSelector(t, t1)));
    }
}