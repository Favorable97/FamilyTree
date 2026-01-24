namespace FamilyTree.API.Errors
{
    public class PersonIsParentException() : DomainException(ErrorCode.PersonHasChildren, "Невозможно удалить человека, так как он является родителем")
    {
    }
}
