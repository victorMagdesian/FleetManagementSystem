namespace FleetManager.Domain.Exceptions;

public class InvalidOperationException : FleetManagerException
{
    public InvalidOperationException(string message) : base(message)
    {
    }

    public InvalidOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
