namespace FleetManager.Domain.Exceptions;

public class EntityNotFoundException : FleetManagerException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID {id} not found")
    {
        EntityName = entityName;
        EntityId = id;
    }

    public string EntityName { get; }
    public Guid EntityId { get; }
}
