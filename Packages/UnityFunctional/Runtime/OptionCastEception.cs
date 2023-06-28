using System;

namespace Bravasoft.Functional
{
    public sealed class OptionCastEception : Exception
    {
        public OptionCastEception() : base("Option is None") { }
    }
}
