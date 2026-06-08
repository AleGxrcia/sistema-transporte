namespace TransportSystem.Core.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Se encontraron errores de validación.")
        {
            Errors = errors;
        }
    }
}
