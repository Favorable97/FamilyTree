using FamilyTree.API.DTO;
using FamilyTree.API.Errors;
using FamilyTree.API.Interfaces;
using FamilyTree.API.Responses;
using FamilyTree.Data.Interfaces;
using FamilyTree.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FamilyTree.API.Services
{
    public class PersonService(IPersonRepository repository, ILifeEventService lifeEvent, ILogger<PersonService> logger) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;
        
        private readonly ILifeEventService _lifeEvent = lifeEvent;

        private readonly ILogger<PersonService> _logger = logger;
        
        public async Task<PersonDTO> CreatePersonAsync(RequestAddPersonDTO requestAddPersonDTO)
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

            var mother = person.MotherID != null ? await _repository.GetPersonByIdAsync(person.MotherID.Value) : null;

            var father = person.FatherID != null ? await _repository.GetPersonByIdAsync(person.FatherID.Value) : null;

            await _lifeEvent.AddEventAsync(
                person.Id,
                LifeEventType.Birth,
                person.BirthDate
            );

            if (person.DeathDate != null)
                await _lifeEvent.AddEventAsync(
                    person.Id,
                    LifeEventType.Death,
                    person.DeathDate.Value
                );

            _logger.LogInformation(
                "Персона успешно добавлена. PersonId: {PersonId}. " +
                "HasMother: {HasMother}. HasFather: {HasFather}. " +
                "HasDeathDate: {HasDeathDate}.",
                person.Id,
                mother != null,
                father != null,
                person.DeathDate != null);

            return PersonMapper.MapToPersonDTO(person, mother, father);
        }

        public async Task<PersonDTO> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO)
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

            if (requestUpdatePersonDTO.DeathDate != null
                && !(await CheckDeathDateForAddEvent(id)))
                await _lifeEvent.AddEventAsync(
                    id,
                    LifeEventType.Death,
                    requestUpdatePersonDTO.DeathDate.Value
                );

            await _repository.UpdatePersonAsync(updatePerson);

            var mother = updatePerson.MotherID != null ? await _repository.GetPersonByIdAsync(updatePerson.MotherID.Value) : null;

            var father = updatePerson.FatherID != null ? await _repository.GetPersonByIdAsync(updatePerson.FatherID.Value) : null;

            _logger.LogInformation(
                "Персона успешно обновлена. " +
                "PersonId: {PersonId}. " +
                "HasMother: {HasMother}. " +
                "HasFather: {HasFather}. " +
                "HasDeathDate: {HasDeathDate}.",
                id,
                mother != null,
                father != null,
                updatePerson.DeathDate != null);

            return PersonMapper.MapToPersonDTO(updatePerson, mother, father);
        }
        
        public async Task<List<ShortPersonDTO>> GetAllPersonAsync()
        {
            List<Person> persons = await _repository.GetAllPersonAsync();
            
            return [.. persons.Select(PersonMapper.MapToShortPersonDTO)];
        }
        
        public async Task<PersonDTO> GetPersonByIdAsync(Guid id)
        {
            Person? person = await _repository.GetPersonByIdAsync(id) ?? throw new PersonNotFoundException(id);

            var mother = person.MotherID == null
                ? null
                : await _repository.GetPersonByIdAsync(person.MotherID.Value);

            var father = person.FatherID == null
                ? null
                : await _repository.GetPersonByIdAsync(person.FatherID.Value);

            var personDTO = PersonMapper.MapToPersonDTO(person, mother, father);

            return personDTO;
        }

        public async Task<ShortPersonDTO> GetShortPersonByIdAsync(Guid id)
        {
            Person? person = await _repository.GetPersonByIdAsync(id) ?? throw new PersonNotFoundException(id);

            var mother = person.MotherID == null
                ? null
                : await _repository.GetPersonByIdAsync(person.MotherID.Value);

            var father = person.FatherID == null
                ? null
                : await _repository.GetPersonByIdAsync(person.FatherID.Value);

            return PersonMapper.MapToShortPersonDTO(person);
        }

        
        public async Task DeletePersonAsync(Guid id)
        {
            var person = await _repository.GetPersonByIdAsync(id) ?? throw new PersonNotFoundException(id);

            await IsParentPerson(id);

            await _repository.DeletePersonAsync(id);

            _logger.LogInformation("Персона успешно удалена. PersonId: {PersonId}.", id);
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

            if (mother.Gender != Gender.Female || mother.BirthDate >= childBirthDate)
                throw new InvalidParentException();
        }

        private async Task ValidationFather(Guid fatherId, DateTime childBirthDate)
        {
            var father = await _repository.GetPersonByIdAsync(fatherId) ?? throw new ParentNotFoundException();

            if (father.Gender != Gender.Male || father.BirthDate >= childBirthDate)
                throw new InvalidParentException();
        }

        private async Task IsParentPerson(Guid id)
        {
            var isParent = await _repository.IsParentAsync(id);

            if (isParent)
                throw new PersonIsParentException();
        }

        private async Task<bool> CheckDeathDateForAddEvent(Guid personId)
        {
            var eventList = await _lifeEvent.GetTimelineAsync(personId);

            return eventList.Exists(ev => ev.Type.Equals("Death", StringComparison.OrdinalIgnoreCase));
        }
        #endregion
    }
}
