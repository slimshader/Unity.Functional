using System;

namespace Bravasoft
{
    public sealed class ResultFailException<TError> : Exception
    {
        public ResultFailException(TError error)
        {
            Error = error;
        }

        public TError Error { get; }
    }
}
