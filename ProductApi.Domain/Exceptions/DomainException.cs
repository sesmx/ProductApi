namespace ProductApi.Domain.Exceptions;

public abstract class DomainException : Exception
{
	protected DomainException(string message) : base(message) { }
}

public sealed class NotFoundException : DomainException
{
	public NotFoundException(string entity, object key)
		: base($"{entity} with key '{key}' was not found.") { }
}

public sealed class DuplicateNameException : DomainException
{
	public DuplicateNameException(string entity, string name)
		: base($"{entity} with name '{name}' already exists.") { }
}

public sealed class InvalidQuantityException(int qty)
	: DomainException($"Quantity '{qty}' must be greater than zero.");
