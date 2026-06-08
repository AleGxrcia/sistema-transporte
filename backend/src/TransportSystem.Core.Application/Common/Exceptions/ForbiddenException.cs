namespace TransportSystem.Core.Application.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string action, string requiredRole)
            : base($"No tiene permisos para '{action}'. Se requiere el rol '{requiredRole}'.")
        {
        }

        public ForbiddenException(string message)
            : base(message) 
        {
        }
    }
}
