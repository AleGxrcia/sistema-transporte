namespace TransportSystem.Core.Application.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string resourceName, string field, object value)
            : base($"Ya existe un {resourceName} con {field} '{value}'.") 
        {
        }

        public ConflictException(string message)
            : base(message) 
        {
        }
    }
}
