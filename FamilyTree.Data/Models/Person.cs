using FamilyTree.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Models
{
    public record Person
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string LastName { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string? MiddleName { get; init; }
        public DateTime BirthDate { get; init; }
        public DateTime? DeathDate { get; init; }
        public Gender Gender { get; init; }
        public Guid? MotherID { get; init; }
        public Guid? FatherID {  get; init; }
    }
}
