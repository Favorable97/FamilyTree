namespace FamilyTree.API.Errors
{
    public abstract class DomainException(ErrorCode errorCode, string message) : Exception(message)
    {
        public ErrorCode ErrorCode { get; } = errorCode;
    }
}
