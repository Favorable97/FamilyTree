using FamilyTree.API.Responses;
using FamilyTree.Data.Models;

namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task<List<Person>> GetAllPersonAsync();
        public Task<PersonDTO?> GetPersonByIdAsync(Guid id);
        public Task<Person> CreatePersonAsync(RequestAddPersonDTO requestAddPersonDTO);
        public Task<Person> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO);
        public Task DeletePersonAsync(Guid id);
    }
}
