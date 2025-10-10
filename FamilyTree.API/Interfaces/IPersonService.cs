namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task AddPerson(RequestAddPersonDTO requestAddPersonDTO);
        public Task SetParentAsync(Guid childId, RequestSetParentDTO requestSetParentDTO);
    }
}
