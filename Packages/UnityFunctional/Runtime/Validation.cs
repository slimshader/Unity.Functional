using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public readonly struct Validation<T>
    {
        private readonly (T Value, IReadOnlyList<Error> Errors) _data;
    }
}
