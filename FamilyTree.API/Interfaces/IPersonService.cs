namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        public Task AddPerson(RequestAddPersonDTO requestAddPersonDTO);
    }
}
