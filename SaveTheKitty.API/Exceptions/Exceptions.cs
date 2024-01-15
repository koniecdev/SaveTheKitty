namespace SaveTheKitty.API.Exceptions;

public sealed class NotFoundException(string entity, string id) : Exception($"Could not found {entity} with identifier: {id}")
{
}
public sealed class UniquePropertyAlreadyExistException(string property, string value) : Exception($"Given {property}: {value} already exists in database")
{
}