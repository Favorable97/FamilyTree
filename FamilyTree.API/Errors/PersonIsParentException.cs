namespace FamilyTree.API.Errors
{
    public class PersonIsParentException() : DomainException("Невозможно удалить человека, так как он является родителем")
    {
        public override ErrorCode ErrorCode => ErrorCode.PersonHasChildren;
    }
}
