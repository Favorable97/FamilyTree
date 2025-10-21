namespace FamilyTree.API.DTO
{
    public record RequestUpdatePersonDTO
    {
        public string? LastName { get; init; }
        public string? FirstName { get; init; }
        public string? MiddleName { get; init; }
        public DateTime? BirthDate { get; init; }
        public DateTime? DeathDate { get; init; }
        public Gender? Gender { get; init; }
        public Guid? MotherID { get; init; }
        public Guid? FatherID { get; init; }
    }
}
