namespace FamilyTree.API.Errors
{
    public class InvalidParentException() : DomainException(ErrorCode.InvalidParent, "Ошибка при попытке связать родителя")
    {
    }
}
