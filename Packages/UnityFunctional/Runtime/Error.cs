namespace Bravasoft.Functional
{
    public class Error
    {
        public Error(string message = default)
        {
            Message = message;
        }

        public virtual string Message { get; }

        public virtual bool IsException => false;

        public virtual void RethrowIfException() { }

        public override string ToString() =>
            $"Error: {GetType().Name}" + (string.IsNullOrWhiteSpace(Message) ? string.Empty : $" ({Message})");
    }
}
