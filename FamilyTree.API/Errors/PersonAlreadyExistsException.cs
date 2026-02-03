namespace FamilyTree.API.Errors
{
    public class PersonAlreadyExistsException() : DomainException("Персона с добавляемыми параметрами уже существует")
    {
        public override ErrorCode ErrorCode => ErrorCode.PersonAlreadyExists;
    }
}
