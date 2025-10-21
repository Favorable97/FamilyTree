namespace FamilyTree.API.Mappers
{
    public static class PersonMapper
    {
        public async static Task<PersonDTO> MapToPersonDTO(Person person, Func<Guid, Task<Person>?> getPersonById)
        {
            PersonDTO motherDTO = null,
                fatherDTO = null;

            if (person.MotherID.HasValue)
            {
                var mother = await getPersonById(person.MotherID.Value);

                if (mother is not null)
                {
                    motherDTO = Map(mother);
                }
            }

            if (person.FatherID.HasValue)
            {
                var father = await getPersonById(person.FatherID.Value);

                if (father is not null)
                {
                    fatherDTO = Map(father);
                }
            }

            return new()
            {
                Id = person.Id,
                LastName = person.LastName,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                BirthDate = person.BirthDate,
                DeathDate = person.DeathDate,
                Gender = person.Gender,
                Mother = motherDTO ?? null,
                Father = fatherDTO ?? null
            };

            static PersonDTO Map(Person person) => new()
            {
                Id = person.Id,
                LastName = person.LastName,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                BirthDate = person.BirthDate,
                DeathDate = person.DeathDate,
                Gender = person.Gender
            };
        }
    }
}
