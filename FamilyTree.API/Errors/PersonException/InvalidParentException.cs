namespace FamilyTree.API.Errors
{
    public class InvalidParentException() : DomainException("Ошибка при попытке связать родителя")
    {
        public override ErrorCode ErrorCode => ErrorCode.InvalidParent;
    }
}

