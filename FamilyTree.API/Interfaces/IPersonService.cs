using FamilyTree.API.Responses;
using FamilyTree.Data.Models;

namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task<List<ShortPersonDTO>> GetAllPersonAsync();
        public Task<PersonDTO> GetPersonByIdAsync(Guid id);
        public Task<PersonDTO> CreatePersonAsync(RequestAddPersonDTO requestAddPersonDTO);
        public Task<PersonDTO> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO);
        public Task DeletePersonAsync(Guid id);
    }
}
