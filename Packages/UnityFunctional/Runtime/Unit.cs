using System;

namespace Bravasoft.Unity.Functional
{
    public readonly struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Default = default;
        public bool Equals(Unit other) => true;
    }
}
