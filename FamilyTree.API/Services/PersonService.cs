using FamilyTree.API.DTO;
using FamilyTree.API.Errors;
using FamilyTree.API.Interfaces;
using FamilyTree.API.Mappers;
using FamilyTree.API.Responses;
using FamilyTree.Data.Interfaces;
using FamilyTree.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Threading.Tasks;

namespace FamilyTree.API.Services
{
    public class PersonService(IPersonRepository repository) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;

        /// <summary>
        /// Сервис по добавлению человека
        /// </summary>
        /// <param name="requestAddPersonDTO">Объект данных о человеке</param>
        /// <returns></returns>
        public async Task<Person> CreatePersonAsync(RequestAddPersonDTO requestAddPersonDTO)
        {
            await СheckExistsPerson(requestAddPersonDTO.LastName, requestAddPersonDTO.FirstName, requestAddPersonDTO.MiddleName, requestAddPersonDTO.BirthDate);

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

            await ParentValidation(person);

            await _repository.CreatePersonAsync(person);

            return person;
        }

        /// <summary>
        /// Сервис по изменению информации о человеке
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <param name="requestUpdatePersonDTO">Информация для изменения</param>
        /// <returns></returns>
        public async Task<Person> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO)
        {
            var personFromDB = await _repository.GetPersonByIdAsync(id) ?? throw new PersonNotFoundException(id);

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

            await ParentValidation(updatePerson);

            await _repository.UpdatePersonAsync(updatePerson);

            return updatePerson;
        }

        /// <summary>
        /// Получение списка всех людей в системе
        /// </summary>
        /// <returns></returns>
        public async Task<List<Person>> GetAllPersonAsync()
        {
            List<Person> persons = await _repository.GetAllPersonAsync();
            
            return persons;
        }

        /// <summary>
        /// Получение человека по Id
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <returns></returns>
        public async Task<PersonDTO?> GetPersonByIdAsync(Guid id)
        {
            Person? person = await _repository.GetPersonByIdAsync(id);

            if (person is null)
                return null;

            var mother = person.MotherID == null
                ? null
                : await _repository.GetPersonByIdAsync(person.MotherID.Value);

            var father = person.FatherID == null
                ? null
                : await _repository.GetPersonByIdAsync(person.FatherID.Value);

            var personDTO = PersonMapper.MapToPersonDTO(person, mother, father);

            return personDTO;
        }
        
        /// <summary>
        /// Удаление человека
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeletePersonAsync(Guid id)
        {
            var person = await _repository.GetPersonByIdAsync(id) ?? throw new PersonNotFoundException(id);

            await IsParentPerson(id);

            await _repository.DeletePersonAsync(id);
        }

        #region Вспомогательные методы
        private async Task СheckExistsPerson(string lastName, string firstName, string? middleName, DateTime birthDate)
        {
            var isExist = await _repository.ExistsAsync(lastName, firstName, middleName, birthDate);

            if (isExist)
                throw new PersonAlreadyExistsException();
        }

        private async Task ParentValidation(Person person)
        {
            if (person.MotherID.HasValue && person.FatherID.HasValue)
            {
                if (person.MotherID == person.FatherID)
                    throw new InvalidParentException();
            }

            if (person.MotherID.HasValue)
                await ValidationMother(person.MotherID.Value, person.BirthDate);

            if (person.FatherID.HasValue)
                await ValidationFather(person.FatherID.Value, person.BirthDate);
        }

        private async Task ValidationMother(Guid motherId, DateTime childBirthDate) 
        { 
            var mother = await _repository.GetPersonByIdAsync(motherId) ?? throw new ParentNotFoundException();

            if (mother.Gender != Gender.Female)
                throw new InvalidParentException();

            if (mother.BirthDate >= childBirthDate)
                throw new InvalidParentException();
        }

        private async Task ValidationFather(Guid fatherId, DateTime childBirthDate)
        {
            var father = await _repository.GetPersonByIdAsync(fatherId) ?? throw new ParentNotFoundException();

            if (father.Gender != Gender.Male)
                throw new InvalidParentException();

            if (father.BirthDate >= childBirthDate)
                throw new InvalidParentException();
        }

        private async Task IsParentPerson(Guid id)
        {
            var isParent = await _repository.IsParentAsync(id);

            if (isParent)
                throw new PersonIsParentException();
        }

        #endregion
    }
}
