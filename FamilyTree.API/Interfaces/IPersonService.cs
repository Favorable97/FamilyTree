using FamilyTree.Data.Models;

namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task<List<Person>> GetAllPersonAsync();
        public Task<PersonDTO?> GetPersonByIdAsync(Guid id);
        public Task AddPersonAsync(RequestAddPersonDTO requestAddPersonDTO);
        public Task UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO);
        public Task SetParentAsync(Guid childId, RequestSetParentDTO requestSetParentDTO);
        public Task DeletePersonAsync(Guid id);
    }
}
