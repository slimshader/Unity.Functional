using System;

namespace Bravasoft.Functional.Errors
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

        public override void RethrowIfException() => throw Exception;

        public override string ToString() => Exception.ToString();
    }

    public static class ExceptionErrorExtensions
    {
        public static ExceptionError ToError(this Exception exception) =>
            new ExceptionError(exception);
    }
}
