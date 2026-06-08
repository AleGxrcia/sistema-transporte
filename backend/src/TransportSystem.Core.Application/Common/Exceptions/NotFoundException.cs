namespace TransportSystem.Core.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} con Id '{key}' no fue encontrado.") 
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
