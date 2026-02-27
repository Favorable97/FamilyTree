namespace FamilyTree.API.DTO
{
    public record RequestAddDivorceDTO
    {
        public Guid MarriageId { get; init; }
        public DateTime DivorceDate { get; init; }
    }
}
