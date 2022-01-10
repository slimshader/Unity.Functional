using System;

namespace Bravasoft.Unity.Functional
{
    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Default = default;
        public bool Equals(Unit other) => true;
    }
}
