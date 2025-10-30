using FamilyTree.API.Responses;
using FamilyTree.Data.Models;

namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task<ApiResponse<List<Person>>> GetAllPersonAsync();
        public Task<ApiResponse<PersonDTO?>> GetPersonByIdAsync(Guid id);
        public Task<ApiResponse<Person>> AddPersonAsync(RequestAddPersonDTO requestAddPersonDTO);
        public Task<ApiResponse<Person>> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO);
        public Task SetParentAsync(Guid childId, RequestSetParentDTO requestSetParentDTO);
        public Task<ApiResponse<object>> DeletePersonAsync(Guid id);
    }
}
