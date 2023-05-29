namespace Bravasoft.Functional
{
    public class Error
    {
        public Error(string message = default)
        {
            Message = message;
        }

        public string Message { get; }

        public virtual bool IsException => false;
    }
}
