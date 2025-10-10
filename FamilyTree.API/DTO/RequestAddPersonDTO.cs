using FamilyTree.Data.Utils;

namespace FamilyTree.API.DTO
{
    public class RequestAddPersonDTO
    {
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public Gender Gender { get; set; }
        public Guid? MotherID { get; set; }
        public Guid? FatherID { get; set; }
    }
}
