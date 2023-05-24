using System.Collections;
using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public struct IndexedEnumerator<T> : IEnumerable<(T Value, int Index)>, IEnumerator<(T Value, int Index)>
    {
        private IEnumerator<T> _source;
        private int _index;

        public IndexedEnumerator<T> GetEnumerator() => this;

        IEnumerator<(T Value, int Index)> IEnumerable<(T Value, int Index)>.GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public (T Value, int Index) Current => (_source.Current, _index);

        object IEnumerator.Current => this.Current;

        public IndexedEnumerator(IEnumerator<T> source)
        {
            _source = source;
            _index = -1;
        }

        public void Dispose() => _source.Dispose();

        public bool MoveNext()
        {
            if (_source.MoveNext())
            {
                ++_index;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
