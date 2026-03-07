using FamilyTree.Data.Interfaces;

namespace FamilyTree.API.Services
{
    public class LifeEventService(ILifeEventRepository repository, IPersonService personService) : ILifeEventService
    {
        private readonly ILifeEventRepository _repository = repository;
        private readonly IPersonService _personService = personService;

        public async Task AddEventAsync(Guid personId, LifeEventType type, DateTime date, string? description = null)
        {
            await ValidationPerson(personId);


            LifeEvent lifeEvent = new()
            {
                Id = Guid.NewGuid(),
                PersonId = personId,
                Type = type,
                Date = date,
                Description = description
            };

            await _repository.AddEventAsync(lifeEvent);
        }

        public async Task<List<LifeEventDTO>> GetTimelineAsync(Guid personId)
        {
            await ValidationPerson(personId);

            List<LifeEvent> events = await _repository.GetByPersonIdAsync(personId);

            return LifeEventMapper.Map(events);
        }

        private async Task ValidationPerson(Guid personId)
        {
            var person = await _personService.GetPersonByIdAsync(personId) ?? throw new PersonNotFoundException(personId);
        }
    }
}
