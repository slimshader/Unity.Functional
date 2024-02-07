using Cysharp.Threading.Tasks;
using System;

namespace Bravasoft.Functional
{
    public static partial class AsyncPrelude
    {
        public static UniTask<Result<T>> AsyncOk<T>(T value) => UniTask.FromResult(Result<T>.Ok(value));
        public static UniTask<Result<T>> AsyncFail<T>(string errorMessage) => UniTask.FromResult(Result<T>.Fail(new Error(errorMessage)));
    }
    public static class AsyncResult
    {
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

        public static async UniTask<U> MatchAsync<T, U>(this UniTask<Result<T>> asyncResult, Func<T, UniTask<U>> onSome, Func<UniTask<U>> onNone)
        {
            var (isOk, value, error) = await asyncResult;

            if (isOk)
            {
                return await onSome(value);
            }

            return await onNone();
        }


        public static async UniTask<Result<T>> Catch<T>(this UniTask<Result<T>> task)
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

        // LINQ

        public static UniTask<Result<U>> Select<T, U>(this UniTask<Result<T>> task, Func<T, U> selector) =>
            task.Map(selector);


        public static UniTask<Result<U>> SelectMany<T, T1, U>(this UniTask<Result<T>> task, Func<T, UniTask<Result<T1>>> selector, Func<T, T1, U> resultSelector) =>
            task.Bind(t => selector(t).Map(t1 => resultSelector(t, t1)));
    }
}