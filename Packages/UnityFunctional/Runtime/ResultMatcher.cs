using System;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public sealed class ResultMatcher<T>
    {
        Action<Result<T>> _onOk;
        List<Func<Result<T>, bool>> _onFails = new();
        Action _onFallback;

        public ResultMatcher<T> OnOk(Action<T> onOk)
        {
            _onOk = r => r.Iter(onOk);
            return this;
        }

        public ResultMatcher<T> OnError(Action<Error> onError)
        {
            _onFails.Add(r => { r.ErrorIter(onError); return true; });

            return this;
        }

        public ResultMatcher<T> OnError<TError>(Action<TError> onError) where TError : Error
        {
            _onFails.Add(r =>
            {
                if (r.TryGetError<TError>(out var e))
                {
                    onError(e);
                    return true;
                }
                return false;
            });

            return this;
        }

        public Unit Match(in Result<T> result)
        {
            bool isMatched = false;

            if (result.IsOk)
            {
                _onOk(result);
                isMatched = true;
            }
            else
            {
                foreach (var fail in _onFails)
                {
                    if (fail(result))
                    {
                        isMatched = true;
                        break;
                    }
                }
            }

            if (!isMatched)
            {
                _onFallback?.Invoke();
            }

            return default;
        }

        public void OnFallback(Action onFallback)
        {
            _onFallback = onFallback;
        }
    }
}
