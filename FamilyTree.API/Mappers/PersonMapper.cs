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
        public static PersonDTO MapToPersonDTO(Person person, Person? mother, Person? father) => new()
        {
            Id = person.Id,
            LastName = person.LastName,
            FirstName = person.FirstName,
            MiddleName = person.MiddleName,
            BirthDate = person.BirthDate,
            DeathDate = person.DeathDate,
            Gender = person.Gender.ToString(),

            Mother = mother == null ? null : MapToShortPersonDTO(mother),

            Father = father == null ? null : MapToShortPersonDTO(father)
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
