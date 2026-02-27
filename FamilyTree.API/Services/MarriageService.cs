using FamilyTree.API.Errors;
using FamilyTree.Data.Interfaces;

namespace FamilyTree.API.Services
{
    public class MarriageService(IMarriageRepository repository, IPersonService personService) : IMarriageService
    {
        private readonly IMarriageRepository _repository = repository;
        private readonly IPersonService _personService = personService;

        public async Task<MarriageDTO> CreateMarriageAsync(RequestAddMarriageDTO dto)
        {
            var spouse1 = await _personService.GetShortPersonByIdAsync(dto.Spouse1Id) ?? throw new PersonNotFoundException(dto.Spouse1Id);
            var spouse2 = await _personService.GetShortPersonByIdAsync(dto.Spouse2Id) ?? throw new PersonNotFoundException(dto.Spouse2Id);

            await CheckSpouses(spouse1, spouse2);

            Marriage marriage = new()
            {
                Spouse1Id = dto.Spouse1Id,
                Spouse2Id = dto.Spouse2Id,
                BeginDate = dto.BeginDate
            };

            await _repository.AddAsync(marriage);

            return MarriageMapper.MapToMarriageDTO(marriage, spouse1, spouse2);
        }

        public async Task<MarriageDTO?> DivorceAsync(RequestAddDivorceDTO dto)
        {
            var marriage = await _repository.GetByIdAsync(dto.MarriageId) ?? throw new MarriageNotFoundException();

            await CheckMarriage(marriage);

            if (dto.DivorceDate < marriage.BeginDate)
                throw new InvalidMarriageDataException("Дата начала брака не может быть больше, чем дата развода");

            marriage.EndDate = dto.DivorceDate;

            await _repository.UpdateAsync(marriage);

            var spouse1 = await _personService.GetShortPersonByIdAsync(marriage.Spouse1Id);
            var spouse2 = await _personService.GetShortPersonByIdAsync(marriage.Spouse2Id);

            return MarriageMapper.MapToMarriageDTO(marriage, spouse1, spouse2);
        }

        public async Task<ShortPersonDTO?> GetCurrentSpouseAsync(Guid personId)
        {
            var marriage = await _repository.GetActiveMarriageAsync(personId);

            if (marriage == null)
                return null;

            var spouse = await _personService.GetShortPersonByIdAsync(marriage.Spouse1Id == personId ? marriage.Spouse2Id :  marriage.Spouse1Id);

            return spouse;
        }

        public async Task<List<MarriageDTO>> GetMarriageHistoryAsync(Guid personId)
        {
            var data = await _repository.GetByPersonIdAsync(personId);

            List<MarriageDTO> marriages = [];

            foreach (var married in data)
            {
                var spouse1 = await _personService.GetShortPersonByIdAsync(married.Spouse1Id);
                var spouse2 = await _personService.GetShortPersonByIdAsync(married.Spouse2Id);

                marriages.Add(MarriageMapper.MapToMarriageDTO(married, spouse1, spouse2));
            }

            return marriages;
        }

        #region Вспомогательные методы

        private async Task CheckSpouses(ShortPersonDTO spouse1, ShortPersonDTO spouse2)
        {
            if (spouse1.Id.Equals(spouse2.Id))
                throw new InvalidMarriageDataException("Супруги не могут быть равны друг другу");

            await CheckSpouse(spouse1.Id);
            await CheckSpouse(spouse2.Id);
        }

        private async Task CheckSpouse(Guid spouseId)
        {
            var married = await _repository.GetActiveMarriageAsync(spouseId);

            if (married != null)
                throw new ActiveMarriageExistsException();
        }

        private async Task CheckMarriage(Marriage marriage)
        {
            if (marriage.EndDate != null)
                throw new InvalidMarriageDataException("Брак уже закрыт");
        }
        #endregion
    }
}
