namespace FamilyTree.API.Errors
{
    public class PersonAlreadyExistsException() : DomainException(ErrorCode.PersonAlreadyExists, "Персона с добавляемыми параметрами уже существует")
    {
    }
}
