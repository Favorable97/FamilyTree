namespace FamilyTree.Data.Models
{
    public record Marriage
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid Spouse1Id { get; init; }
        public Guid Spouse2Id { get; init; }

        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MarriageEndReason? EndReason { get; set; }
    }
}
