using FamilyTree.API.DTO;
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
        public async Task<ApiResponse<Person>> CreatePersonAsync(RequestAddPersonDTO requestAddPersonDTO)
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

            return ApiResponse<Person>.Ok(person, "Человек успешно добавлен");
        }

        /// <summary>
        /// Сервис по изменению информации о человеке
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <param name="requestUpdatePersonDTO">Информация для изменения</param>
        /// <returns></returns>
        public async Task<ApiResponse<Person>> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO)
        {
            var personFromDB = await _repository.GetPersonByIdAsync(id) ?? throw new Exception($"Человек с Id: {id} не найден");

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

            return ApiResponse<Person>.Ok(updatePerson, "Информация о человеке успешно обновлена");
        }

        /// <summary>
        /// Получение списка всех людей в системе
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse<List<Person>>> GetAllPersonAsync()
        {
            List<Person> persons = await _repository.GetAllPersonAsync();
            
            return ApiResponse<List<Person>>.Ok(persons, "");
        }

        /// <summary>
        /// Получение человека по Id
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Удаление человека
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResponse<object>> DeletePersonAsync(Guid id)
        {
            var person = await _repository.GetPersonByIdAsync(id) ?? throw new Exception($"Человек с Id: {id} не найден!");

            await IsParentPerson(id);

            await _repository.DeletePersonAsync(id);

            return ApiResponse<object>.Ok(null, "Человек успешно удален");
        }

        #region Вспомогательные методы
        private async Task СheckExistsPerson(string lastName, string firstName, string? middleName, DateTime birthDate)
        {
            var isExist = await _repository.ExistsAsync(lastName, firstName, middleName, birthDate);

            if (isExist)
                throw new Exception("Попытка повторного добавления персоны!");
        }

        private async Task ParentValidation(Person person)
        {
            if (person.MotherID.HasValue && person.FatherID.HasValue)
            {
                if (person.MotherID == person.FatherID)
                    throw new Exception("Ссылка на мать совпадает с ссылкой на отца!");
            }

            if (person.MotherID.HasValue)
                await ValidationMother(person.MotherID.Value, person.BirthDate);

            if (person.FatherID.HasValue)
                await ValidationFather(person.FatherID.Value, person.BirthDate);
        }

        private async Task ValidationMother(Guid motherId, DateTime childBirthDate) 
        { 
            var mother = await _repository.GetPersonByIdAsync(motherId);

            if (mother == null)
                throw new Exception("Выбрана несуществующая персона!");

            if (mother.Gender != Gender.Female)
                throw new Exception("Выбранная персона - не женщина и не может быть матерью!");

            if (mother.BirthDate >= childBirthDate)
                throw new Exception("Ребенок не может быть старше матери!");
        }

        private async Task ValidationFather(Guid fatherId, DateTime childBirthDate)
        {
            var father = await _repository.GetPersonByIdAsync(fatherId);

            if (father == null)
                throw new Exception("Выбрана несуществующая персона!");

            if (father.Gender != Gender.Male)
                throw new Exception("Выбранная персона - не мужчина и не может быть папой!");

            if (father.BirthDate >= childBirthDate)
                throw new Exception("Ребенок не может быть старше отца!");
        }

        private async Task IsParentPerson(Guid id)
        {
            var isParent = await _repository.IsParentAsync(id);

            if (isParent)
                throw new Exception("Невозможно удалить человека, так как он является родителем!");
        }

        #endregion
    }
}
