namespace FamilyTree.API.DTO
{
    public record RequestAddMarriageDTO
    {
        public Guid Spouse1Id { get; init; }
        public Guid Spouse2Id { get; init; }

        public DateTime BeginDate { get; init; }
        public DateTime? EndDate { get; init; }
    }
}
