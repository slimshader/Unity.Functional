using System;

namespace UnityFunctional
{
    public sealed class ResultFailException<TError> : Exception
    {
        public ResultFailException(TError error) : base($"Result Fail with {error}")
        {
            Error = error;
        }

        public TError Error { get; }
    }
}
