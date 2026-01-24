namespace FamilyTree.API.Errors
{
    public sealed class PersonNotFoundException(Guid id) : DomainException(ErrorCode.PersonNotFound, $"Персона с id = {id} не найдена") { };
}
