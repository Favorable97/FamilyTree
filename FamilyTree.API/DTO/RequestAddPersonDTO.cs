using FamilyTree.Data.Utils;

namespace FamilyTree.API.DTO
{
    public record RequestAddPersonDTO
    {
        public string LastName { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string? MiddleName { get; init; }
        public DateTime BirthDate { get; init; }
        public DateTime? DeathDate { get; init; }
        public Gender Gender { get; init; }
        public Guid? MotherID { get; init; }
        public Guid? FatherID { get; init; }
    }
}
