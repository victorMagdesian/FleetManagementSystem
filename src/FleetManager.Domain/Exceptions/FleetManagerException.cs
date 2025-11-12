namespace FleetManager.Domain.Exceptions;

public class FleetManagerException : Exception
{
    public FleetManagerException(string message) : base(message)
    {
    }

    public FleetManagerException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
