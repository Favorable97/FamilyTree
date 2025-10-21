using FamilyTree.API.Interfaces;
using FamilyTree.Data.Models;
using FamilyTree.Data.Interfaces;
using FamilyTree.API.Mappers;
using Microsoft.IdentityModel.Tokens;
using System.Data;

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

        public async Task UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO)
        {
            var personFromDB = await _repository.GetPersonByIdAsync(id);

            Person updatePerson = new()
            {
                Id = personFromDB!.Id,
                LastName = !string.IsNullOrEmpty(requestUpdatePersonDTO.LastName) ? requestUpdatePersonDTO.LastName : personFromDB.LastName,
                FirstName = !string.IsNullOrEmpty(requestUpdatePersonDTO.FirstName) ? requestUpdatePersonDTO.FirstName : personFromDB.FirstName,
                MiddleName = !string.IsNullOrEmpty(requestUpdatePersonDTO.MiddleName) ? requestUpdatePersonDTO.MiddleName : personFromDB.MiddleName,
                BirthDate = requestUpdatePersonDTO.BirthDate ?? personFromDB.BirthDate,
                DeathDate = requestUpdatePersonDTO.DeathDate ?? personFromDB.DeathDate,
                Gender = requestUpdatePersonDTO.Gender ?? personFromDB.Gender,
                MotherID = requestUpdatePersonDTO.MotherID ?? personFromDB.MotherID,
                FatherID = requestUpdatePersonDTO.FatherID ?? personFromDB.FatherID
            };

            await _repository.UpdatePersonAsync(updatePerson);
        }

        public async Task<List<Person>> GetAllPersonAsync()
        {
            List<Person> persons = await _repository.GetAllPersonAsync();
            return persons;
        }
        public async Task<PersonDTO?> GetPersonByIdAsync(Guid id)
        {
            Person? person = await _repository.GetPersonByIdAsync(id);

            return await PersonMapper.MapToPersonDTO(person, _repository.GetPersonByIdAsync!);
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
