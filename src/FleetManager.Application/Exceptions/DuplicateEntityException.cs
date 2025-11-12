namespace FleetManager.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to create an entity that already exists.
/// </summary>
public class DuplicateEntityException : FleetManagerException
{
    public DuplicateEntityException(string entityName, string field, string value)
        : base($"{entityName} with {field} '{value}' already exists")
    {
    }

    public DuplicateEntityException(string message)
        : base(message)
    {
    }
}
