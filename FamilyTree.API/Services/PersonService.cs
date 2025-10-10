using FamilyTree.API.Interfaces;
using FamilyTree.Data.Models;

namespace FamilyTree.API.Services
{
    public class PersonService(IPersonRepository repository) : IPersonService
    {
        private readonly IPersonRepository _repository = repository;

        public async Task AddPerson(RequestAddPersonDTO requestAddPersonDTO)
        {
            // Todo Сделать валидацию входных данных
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

        public async Task SetParentAsync(Guid childId, RequestSetParentDTO requestSetParentDTO)
        {
            await _repository.SetParentAsync(childId, requestSetParentDTO.ParentId, requestSetParentDTO.ParentRelation);
        }
    }
}
