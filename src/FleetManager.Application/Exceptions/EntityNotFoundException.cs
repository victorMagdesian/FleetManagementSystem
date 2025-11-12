namespace FleetManager.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class EntityNotFoundException : FleetManagerException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID {id} not found")
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }
}
