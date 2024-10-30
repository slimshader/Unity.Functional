using System;
using System.Collections;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public struct OptionEnumerator<T> : IEnumerator<T>
    {
        private readonly Option<T> _option;
        private bool _wasRead;

        public OptionEnumerator(in Option<T> option)
        {
            _option = option;
            _wasRead = false;
        }
        T IEnumerator<T>.Current =>
            _wasRead && _option.IsSome? (T) _option : throw new InvalidOperationException();

        object IEnumerator.Current => ((IEnumerator<T>)this).Current;
        bool IEnumerator.MoveNext()
        {
            if (_option.IsSome && !_wasRead)
            {
                _wasRead = true;
                return true;
            }
            return false;
        }

        void IDisposable.Dispose() { }

        void IEnumerator.Reset()
        {
            _wasRead = false;
        }
    }
}
