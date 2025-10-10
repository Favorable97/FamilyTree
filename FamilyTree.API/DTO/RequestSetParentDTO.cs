namespace FamilyTree.API.DTO
{
    public record RequestSetParentDTO
    {
        public Guid ParentId { get; init; }
        public ParentRelation ParentRelation { get; init; }
    }
}
