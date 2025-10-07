using FamilyTree.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Models
{
    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public DateTime BirhDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public Gender Gender { get; set; }
        public Guid? MotherID { get; set; }
        public Guid? FatherID {  get; set; }
    }
}
