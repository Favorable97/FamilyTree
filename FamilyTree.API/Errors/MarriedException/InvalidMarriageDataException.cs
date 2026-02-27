namespace FamilyTree.API.Errors
{
    public class InvalidMarriageDataException(string message) : DomainException(message)
    {
        public override ErrorCode ErrorCode => ErrorCode.InvalidMarriageData;
    }
}
