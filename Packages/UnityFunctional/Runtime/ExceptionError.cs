using System;

namespace Bravasoft.Unity.Functional
{
    public class ExceptionError : Error
    {
        public ExceptionError(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
