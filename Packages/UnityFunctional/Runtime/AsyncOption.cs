using Cysharp.Threading.Tasks;
using System;

namespace Bravasoft.Functional
{
    public readonly struct AsyncOption<T>
    {
        private readonly UniTask<Option<T>> _asyncOption;

        public static AsyncOption<T> Some(T value) => new AsyncOption<T>(UniTask.FromResult(Option<T>.Some(value)));
        public static readonly AsyncOption<T> None = new AsyncOption<T>(UniTask.FromResult(Option<T>.None));

        public UniTask<bool> IsSome => _asyncOption.Map(option => option.IsSome);
        public UniTask<bool> IsNone => _asyncOption.Map(option => option.IsNone);

        public AsyncOption<U> Map<U>(Func<T, U> map) =>
            new(_asyncOption.Map(option => option.Map(map)));
        
        public AsyncOption<U> Bind<U>(Func<T, AsyncOption<U>> bind)
        {
            var asyncOptionU = _asyncOption.Bind(option => option.Match(
                onSome: value => bind(value)._asyncOption,
                onNone: () => AsyncOption<U>.None._asyncOption));

            return new AsyncOption<U>(asyncOptionU);
        }

        public UniTask<Option<T>>.Awaiter GetAwaiter() => _asyncOption.GetAwaiter();

        public static implicit operator AsyncOption<T>(in Core.NoneType _) => None;

        private AsyncOption(UniTask<Option<T>> uniTask)
        {
            _asyncOption = uniTask;
        }
    }

    public static class AsyncOption
    {
        public static AsyncOption<T> Select<T>(this AsyncOption<T> option, Func<T, T> map) => option.Map(map);
    }
}

