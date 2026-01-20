namespace FamilyTree.API.DTO
{
    public record PersonTreeNodeDTO
    {
        public ShortPersonDTO Person { get; init; } = null!;

        /// <summary>
        /// Родители человека
        /// </summary>
        public List<PersonTreeNodeDTO> Parents { get; init; } = [];

        /// <summary>
        /// Дети человека
        /// </summary>
        public List<PersonTreeNodeDTO> Children { get; init; } = [];
    }
}
