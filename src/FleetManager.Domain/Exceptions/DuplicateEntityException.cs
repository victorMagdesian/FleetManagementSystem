namespace FleetManager.Domain.Exceptions;

public class DuplicateEntityException : FleetManagerException
{
    public DuplicateEntityException(string entityName, string field, string value)
        : base($"{entityName} with {field} '{value}' already exists")
    {
        EntityName = entityName;
        Field = field;
        Value = value;
    }

    public string EntityName { get; }
    public string Field { get; }
    public string Value { get; }
}
