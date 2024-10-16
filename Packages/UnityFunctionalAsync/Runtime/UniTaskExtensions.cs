using Cysharp.Threading.Tasks;
using System;

namespace Bravasoft.Functional.Async
{
    public static class UniTaskExtensions
    {
        public static UniTask<U> Map<T, U>(this in UniTask<T> task, Func<T, U> map)
        {
            return task.ContinueWith(t => map(t));
        }

        public static UniTask<U> Bind<T, U>(this in UniTask<T> task, Func<T, UniTask<U>> bind)
        {
            return task.ContinueWith(t => bind(t));
        }
    }
}
