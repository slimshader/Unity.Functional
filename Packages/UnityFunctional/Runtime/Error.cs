﻿namespace Bravasoft.Functional
{
    public class Error
    {
        public static readonly Error Default = new Error("Unknown");

        public Error(string message = default)
        {
            Message = message;
        }

        public virtual string Message { get; }

        public virtual bool IsException => false;

        public override string ToString() =>
            $"Error: {GetType().Name}" + (string.IsNullOrWhiteSpace(Message) ? string.Empty : $" ({Message})");
    }

    public sealed class FilterError : Error
    {
        public static new readonly FilterError Default = new FilterError();
        public FilterError() : base("Filter error") { }
    }
}
