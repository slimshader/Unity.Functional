namespace Bravasoft.Unity.Functional
{
    public class Error
    {
        public Error(string message = default)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
