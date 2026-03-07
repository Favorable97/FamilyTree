namespace FamilyTree.API.DTO
{
    public record LifeEventDTO
    {
        public string Type { get; init; }
        public DateTime Date { get; init; }
        public string? Desciption { get; init; }
    }
}
