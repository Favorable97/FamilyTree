using FamilyTree.API.Interfaces;
using FamilyTree.Data.Models;
using FamilyTree.Data.Interfaces;

namespace FamilyTree.API.Services
{
    public class PersonService(IPersonRepository repository) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;

        // Todo Сделать валидацию входных данных везде и обработку ошибок

        public async Task AddPersonAsync(RequestAddPersonDTO requestAddPersonDTO)
        {
            var person = new Person()
            {
                LastName = requestAddPersonDTO.LastName,
                FirstName = requestAddPersonDTO.FirstName,
                MiddleName = requestAddPersonDTO.MiddleName,
                BirthDate = requestAddPersonDTO.BirthDate,
                DeathDate = requestAddPersonDTO.DeathDate,
                Gender = requestAddPersonDTO.Gender,
                MotherID = requestAddPersonDTO.MotherID,
                FatherID = requestAddPersonDTO.FatherID
            };

            await _repository.AddPersonAsync(person);
        }

        public async Task<List<Person>> GetAllPersonAsync()
        {
            List<Person> persons = await _repository.GetAllPersonAsync();
            return persons;
        }
        public async Task<PersonDTO> GetPersonByIdAsync(Guid id)
        {
            var person = await _repository.GetPersonByIdAsync(id);

            PersonDTO motherDTO = null,
                fatherDTO = null;

            if (person.MotherID.HasValue)
            {
                var mother = await _repository.GetPersonByIdAsync(person.MotherID.Value);

                if (mother is not null)
                {
                    motherDTO = Map(mother);
                }
            }

            if (person.FatherID.HasValue)
            {
                var father = await _repository.GetPersonByIdAsync(person.FatherID.Value);

                if (father is not null)
                {
                    fatherDTO = Map(father);
                }
            }

            var personDTO = new PersonDTO()
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

            return personDTO;
        }
        public async Task SetParentAsync(Guid childId, RequestSetParentDTO requestSetParentDTO)
        {
            await _repository.SetParentAsync(childId, requestSetParentDTO.ParentId, requestSetParentDTO.ParentRelation);
        }

        public async Task DeletePersonAsync(Guid id)
        {
            await _repository.DeletePersonAsync(id);
        }
    }
}
