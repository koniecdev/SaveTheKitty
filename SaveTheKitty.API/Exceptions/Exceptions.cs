namespace SaveTheKitty.API.Exceptions;

public class NotFoundException(string entity, string id) : Exception($"Could not found {entity} with identifier: {id}"){}
public sealed class UserNotFoundException(string id) : NotFoundException("User", id){}
public sealed class CatNotFoundException(string id) : NotFoundException("Cat", id){}
public sealed class UniquePropertyAlreadyExistException(string property, string value) : Exception($"Given unique property - {property}: {value} already exists in database"){}