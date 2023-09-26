using System;

namespace Bravasoft.Functional
{
    public sealed class ResultCastException : Exception
    {
        public ResultCastException(Error error) : base($"Result Fail with {error}")
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        public Error Error { get; }
    }
}
