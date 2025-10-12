using FamilyTree.Data.Models;

namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task<List<Person>> GetAllPersonAsync();
        public Task AddPerson(RequestAddPersonDTO requestAddPersonDTO);
        public Task SetParentAsync(Guid childId, RequestSetParentDTO requestSetParentDTO);
        public Task DeletePersonAsync(Guid id);
    }
}
