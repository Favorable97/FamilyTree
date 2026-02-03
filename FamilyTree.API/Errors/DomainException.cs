namespace FamilyTree.API.Errors
{
    public abstract class DomainException(string message) : Exception(message)
    {
        public abstract ErrorCode ErrorCode { get; }
    }
}
