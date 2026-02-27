namespace FamilyTree.API.Errors
{
    public class ParentNotFoundException() : DomainException("Выбранный родитель не существует")
    {
        public override ErrorCode ErrorCode => ErrorCode.ParentNotFound;
    }
}