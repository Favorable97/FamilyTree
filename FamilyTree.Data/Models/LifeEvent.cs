namespace FamilyTree.Data.Models
{
    public record LifeEvent
    {
        public Guid Id { get; init; }
        public Guid PersonId { get; init; }
        public LifeEventType Type {  get; init; }
        public DateTime Date { get; init; }
        public string? Description { get; init; }
    }
}
