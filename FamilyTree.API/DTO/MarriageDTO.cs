namespace FamilyTree.API.DTO
{
    public record MarriageDTO
    {
        public Guid Id { get; set; }

        public ShortPersonDTO Spouse1 { get; init; }
        public ShortPersonDTO Spouse2 { get; init; }

        public DateTime BeginDate { get; init; }
        public DateTime? EndDate { get; init; }
        public MarriageEndReason? EndReason { get; init; }
    }
}
