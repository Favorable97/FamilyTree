using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Utils
{
    public static class ConvertFromDataTableToList
    {
        public static List<Person> ConvertToListPerson(this DataTable table)
        {
            List<Person> list = [];

            foreach (DataRow dr in table.Rows)
            {
                var person = new Person()
                {
                    Id = (Guid)dr["Id"],
                    LastName = dr["LastName"].ToString()!,
                    FirstName = dr["FirstName"].ToString()!,
                    MiddleName = dr["MiddleName"].ToString(),
                    BirthDate = DateTime.Parse(dr["BirthDate"].ToString()!),
                    DeathDate = !DateTime.TryParse(dr["DeathDate"].ToString(), out DateTime dateDeath) ? null : dateDeath,
                    Gender = (Gender)dr["GenderId"],
                    MotherID = !Guid.TryParse(dr["MotherId"].ToString(), out Guid motherId) ? null : motherId,
                    FatherID = !Guid.TryParse(dr["FatherId"].ToString(), out Guid fatherId) ? null : fatherId,
                };

                list.Add(person);
            }

            return list;
        }
    }
}
