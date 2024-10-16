using System.Collections.Generic;

namespace Bravasoft.Functional
{
    public class CompundError : Error
    {
        private List<Error> _errors = new();

        public IReadOnlyList<Error> Errors => _errors;

        public override string Message => "Multiple errors";

        public CompundError(Error error)
        {
            if (error is null) throw new System.ArgumentNullException(nameof(error));

            _errors.Add(error);
        }

        public CompundError Add(Error error)
        {
            if (error is null) throw new System.ArgumentNullException(nameof(error));
            _errors.Add(error);
            return this;
        }
    }
}
