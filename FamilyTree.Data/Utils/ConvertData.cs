using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Utils
{
    public static class ConvertData
    {
        public static List<Person> ConvertToListPerson(DataTable table)
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
                    Gender = (Gender)dr["Gender"],
                    MotherID = !Guid.TryParse(dr["MotherId"].ToString(), out Guid motherId) ? null : motherId,
                    FatherID = !Guid.TryParse(dr["FatherId"].ToString(), out Guid fatherId) ? null : fatherId,
                };

                list.Add(person);
            }

            return list;
        }

        public static List<Marriage> ConvertToListMarriage(DataTable table)
        {
            List<Marriage> list = [];

            foreach (DataRow dr in table.Rows)
            {
                var marriage = new Marriage()
                {
                    Id = (Guid)dr["Id"],
                    Spouse1Id = (Guid)dr["Spouse1Id"],
                    Spouse2Id = (Guid)dr["Spouse2Id"],
                    BeginDate = (DateTime)dr["BeginDate"],
                    EndDate = dr["EndDate"] == DBNull.Value
                        ? null
                        : (DateTime)dr["EndDate"]
                };

                list.Add(marriage);
            }

            return list;
        }

        public static List<LifeEvent> ConvertToListLifeEvent(DataTable table)
        {
            List<LifeEvent> list = [];

            foreach (DataRow dr in table.Rows)
            {
                var lifeEvent = new LifeEvent()
                {
                    Id = (Guid)dr["Id"],
                    PersonId = (Guid)dr["PersonId"],
                    Type = (LifeEventType)dr["Type"],
                    Date = (DateTime)dr["Date"],
                    Description = dr["Description"].ToString() ?? ""
                };

                list.Add(lifeEvent);
            }

            return list;
        }

        /*public static Person ConvertToPersonWithParent(DataTable table)
        {
            DataRow row = table.Rows[0];
            var person = new Person
            {
                Id = row.Field<Guid>("Person_Id"),
                FirstName = row.Field<string>("Person_FirstName")!,
                LastName = row.Field<string>("Person_LastName")!,
                MiddleName = row.Field<string?>("Person_MiddleName"),
                BirthDate = row.Field<DateTime>("Person_BirthDate"),
                DeathDate = row.Field<DateTime?>("Person_DeathDate"),
                Gender = (Gender)row.Field<int>("Person_Gender"),

                Father = row.IsNull("Father_Id") ? null : new Person
                {
                    Id = row.Field<Guid>("Father_Id"),
                    FirstName = row.Field<string>("Father_FirstName")!,
                    LastName = row.Field<string>("Father_LastName")!,
                    MiddleName = row.Field<string?>("Father_MiddleName"),
                    BirthDate = row.Field<DateTime?>("Father_BirthDate"),
                    DeathDate = row.Field<DateTime?>("Father_DeathDate"),
                    Gender = (Gender)row.Field<int>("Father_Gender")
                },

                Mother = row.IsNull("Mother_Id") ? null : new Person
                {
                    Id = row.Field<Guid>("Mother_Id"),
                    FirstName = row.Field<string>("Mother_FirstName")!,
                    LastName = row.Field<string>("Mother_LastName")!,
                    MiddleName = row.Field<string?>("Mother_MiddleName"),
                    BirthDate = row.Field<DateTime?>("Mother_BirthDate"),
                    DeathDate = row.Field<DateTime?>("Mother_DeathDate"),
                    Gender = (Gender)row.Field<int>("Mother_Gender")
                }
            }
        }*/
    }
}
