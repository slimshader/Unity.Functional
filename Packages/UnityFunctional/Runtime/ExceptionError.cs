using System;

namespace Bravasoft.Functional
{
    public class ExceptionError : Error
    {
        public ExceptionError(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public Exception Exception { get; }

        public override string Message => Exception.Message;

        public override bool IsException => true;

        public override string ToString() => Exception.ToString();
    }
}
