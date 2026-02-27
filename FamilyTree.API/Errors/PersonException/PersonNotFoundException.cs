namespace FamilyTree.API.Errors
{
    public sealed class PersonNotFoundException(Guid id) : DomainException($"Персона с id = {id} не найдена")
    {
        public override ErrorCode ErrorCode => ErrorCode.PersonNotFound;
    }
}
