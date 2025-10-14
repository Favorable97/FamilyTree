namespace FamilyTree.API.DTO
{
    public class RequestUpdatePersonDTO
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public Gender? Gender { get; set; }
        public Guid? MotherID { get; set; }
        public Guid? FatherID { get; set; }
    }
}
