namespace FamilyTree.API.Mappers
{
    public static class PersonMapper
    {
        /// <summary>
        /// Преобразуем из объектов Person в объект PersonDTO
        /// </summary>
        /// <param name="person">Персона, которую искали</param>
        /// <param name="mother">Объект матери</param>
        /// <param name="father">Объект отца</param>
        /// <returns></returns>
        public static PersonDTO MapToPersonDTO(Person person, Person? mother, Person? father) => new PersonDTO()
        {
            Id = person.Id,
            LastName = person.LastName,
            FirstName = person.FirstName,
            MiddleName = person.MiddleName,
            BirthDate = person.BirthDate,
            DeathDate = person.DeathDate,
            Gender = person.Gender,

            Mother = mother == null ? null : new ShortPersonDTO()
            {
                Id = mother.Id,
                LastName = mother.LastName,
                FirstName = mother.FirstName,
                MiddleName = mother.MiddleName,
                BirthDate = mother.BirthDate,
                DeathDate = mother.DeathDate
            },

            Father = father == null ? null : new ShortPersonDTO()
            {
                Id = father.Id,
                LastName = father.LastName,
                FirstName = father.FirstName,
                MiddleName = father.MiddleName,
                BirthDate = father.BirthDate,
                DeathDate = father.DeathDate
            }
        };

        public static ShortPersonDTO MapToShortPersonDTO(Person person) => new()
        {
            Id = person.Id,
            LastName = person.LastName,
            FirstName = person.FirstName,
            MiddleName = person.MiddleName,
            BirthDate = person.BirthDate,
            DeathDate = person.DeathDate
        };
    }
}
