namespace FamilyTree.API.Errors
{
    public class MarriageNotFoundException() : DomainException("Информация о браке не найдена")
    {
        public override ErrorCode ErrorCode => ErrorCode.MarriageNotFound;
    }
}