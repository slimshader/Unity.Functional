using System;

namespace Bravasoft.Functional
{
    public struct OptionEnumerator<T>
    {
        private readonly Option<T> _option;
        private bool _wasRead;

        public OptionEnumerator(in Option<T> option)
        {
            _option = option;
            _wasRead = false;
        }

        public T Current => _wasRead && _option.IsSome ? (T)_option : throw new InvalidOperationException();

        public bool MoveNext()
        {
            if (_option.IsSome && !_wasRead)
            {
                _wasRead = true;
                return true;
            }
            return false;
        }
    }
}
