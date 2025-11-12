namespace FleetManager.Application.Exceptions;

/// <summary>
/// Base exception class for FleetManager application errors.
/// </summary>
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
