using System.Collections.Generic;

namespace Bravasoft.Unity.Functional
{
    public struct IndexedEnumerator<T>
    {
        private IEnumerator<T> _source;
        private int _index;

        public (T Value, int Index) Current => (_source.Current, _index);

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

        public IndexedEnumerator<T> GetEnumerator() => this;
    }

    public static class IndexedLinqExtensions
    {
        public static IndexedEnumerator<T> Indexed<T>(this IEnumerable<T> ts) =>
            new IndexedEnumerator<T>(ts.GetEnumerator());
    }
}
