namespace ClyvoVetChallenge.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"Entidade '{entityName}' com identificador '{key}' nao foi encontrada.")
    {
    }
}
