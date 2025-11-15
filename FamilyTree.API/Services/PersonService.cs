using FamilyTree.API.Interfaces;
using FamilyTree.Data.Models;
using FamilyTree.Data.Interfaces;
using FamilyTree.API.Mappers;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using FamilyTree.API.Responses;

namespace FamilyTree.API.Services
{
    public class PersonService(IPersonRepository repository) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;

        // Todo Сделать валидацию входных данных везде и обработку ошибок

        public async Task<ApiResponse<Person>> AddPersonAsync(RequestAddPersonDTO requestAddPersonDTO)
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

            return ApiResponse<Person>.Ok(person, "Человек успешно добавлен");
        }

        public async Task<ApiResponse<Person>> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO)
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

            return ApiResponse<Person>.Ok(updatePerson, "Информация о человеке успешно обновлена");
        }

        public async Task<ApiResponse<List<Person>>> GetAllPersonAsync()
        {
            List<Person> persons = await _repository.GetAllPersonAsync();
            
            return ApiResponse<List<Person>>.Ok(persons, "");
        }
        public async Task<ApiResponse<PersonDTO?>> GetPersonByIdAsync(Guid id)
        {
            Person? person = await _repository.GetPersonByIdAsync(id);

            var motherTask = person.MotherID == null
                ? Task.FromResult<Person?>(null)
                : _repository.GetPersonByIdAsync(person.MotherID.Value);
            var fatherTask = person.FatherID == null
                ? Task.FromResult<Person?>(null)
                : _repository.GetPersonByIdAsync(person.FatherID.Value);

            await Task.WhenAll(motherTask, fatherTask);

            var personDto = PersonMapper.MapToPersonDTO(person, motherTask.Result, fatherTask.Result);

            return ApiResponse<PersonDTO?>.Ok(personDto, "");
        }
        public async Task<ApiResponse<object>> DeletePersonAsync(Guid id)
        {
            await _repository.DeletePersonAsync(id);

            return ApiResponse<object>.Ok(null, "Человек успешно удален");
        }
    }
}
