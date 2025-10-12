using FamilyTree.API.Interfaces;
using FamilyTree.Data.Models;
using FamilyTree.Data.Interfaces;

namespace FamilyTree.API.Services
{
    public class PersonService(IPersonRepository repository) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;

        // Todo Сделать валидацию входных данных везде и обработку ошибок

        public async Task AddPerson(RequestAddPersonDTO requestAddPersonDTO)
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
