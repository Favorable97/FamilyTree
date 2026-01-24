namespace FamilyTree.API.Errors
{
    public class ParentNotFoundException() : DomainException(ErrorCode.ParentNotFound, "Выбранный родитель не существует")
    {
    }
}
