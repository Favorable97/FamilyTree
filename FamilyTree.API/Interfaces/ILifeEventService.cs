namespace FamilyTree.API.Interfaces
{
    public interface ILifeEventService
    {
        Task AddEventAsync(Guid personId, LifeEventType type, DateTime date, string? description = null);
        Task<List<LifeEventDTO>> GetTimelineAsync(Guid personId);
    }
}
