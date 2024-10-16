using System;

namespace Bravasoft.Functional
{
    public sealed class OptionCastException : Exception
    {
        public OptionCastException() : base("Option is None") { }
    }
}
