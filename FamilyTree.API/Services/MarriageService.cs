using FamilyTree.API.Errors;
using FamilyTree.Data.Interfaces;

namespace FamilyTree.API.Services
{
    public class MarriageService(
        IMarriageRepository repository, 
        IPersonService personService, 
        ILifeEventService lifeEvent, 
        IUnitOfWork unitOfWork, 
        ILogger<MarriageService> logger) 
        : IMarriageService
    {
        private readonly IMarriageRepository _repository = repository;

        private readonly IPersonService _personService = personService;

        private readonly ILifeEventService _lifeEventService = lifeEvent;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private readonly ILogger<MarriageService> _logger = logger;

        public async Task<MarriageDTO> CreateMarriageAsync(RequestAddMarriageDTO dto)
        {
            var spouse1 = await _personService.GetShortPersonByIdAsync(dto.Spouse1Id) ?? throw new PersonNotFoundException(dto.Spouse1Id);
            var spouse2 = await _personService.GetShortPersonByIdAsync(dto.Spouse2Id) ?? throw new PersonNotFoundException(dto.Spouse2Id);

            await CheckSpouses(spouse1, spouse2, dto.BeginDate);

            Marriage marriage = new()
            {
                Spouse1Id = dto.Spouse1Id,
                Spouse2Id = dto.Spouse2Id,
                BeginDate = dto.BeginDate
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _repository.AddAsync(marriage);

                await _lifeEventService.AddEventAsync(
                    dto.Spouse1Id,
                    LifeEventType.Marriage,
                    dto.BeginDate,
                    $"Брак с {GetFullName(spouse2.LastName, spouse2.FirstName, spouse2.MiddleName)}"
                );

                await _lifeEventService.AddEventAsync(
                    dto.Spouse2Id,
                    LifeEventType.Marriage,
                    dto.BeginDate,
                    $"Брак с {GetFullName(spouse1.LastName, spouse1.FirstName, spouse1.MiddleName)}"
                );

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Брак успешно создан. " +
                    "Spouse1Id: {Spouse1Id}. " +
                    "Spouse2Id: {Spouse2Id}. " +
                    "LifeEventType: {LifeEventType}. " +
                    "BeginDate: {BeginDate}.",
                    spouse1.Id,
                    spouse2.Id,
                    LifeEventType.Marriage,
                    dto.BeginDate);

                return MarriageMapper.MapToMarriageDTO(marriage, spouse1, spouse2);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
        }

        public async Task<MarriageDTO?> DivorceAsync(RequestAddDivorceDTO dto)
        {
            var marriage = await _repository.GetByIdAsync(dto.MarriageId) ?? throw new MarriageNotFoundException();

            var spouse1 = await _personService.GetShortPersonByIdAsync(marriage.Spouse1Id) ?? throw new PersonNotFoundException(marriage.Spouse1Id);
            var spouse2 = await _personService.GetShortPersonByIdAsync(marriage.Spouse2Id) ?? throw new PersonNotFoundException(marriage.Spouse2Id);

            await CheckMarriage(marriage);

            if (dto.DivorceDate < marriage.BeginDate)
                throw new InvalidMarriageDataException("Дата начала брака не может быть больше, чем дата развода");

            marriage.EndDate = dto.DivorceDate;
            marriage.EndReason = dto.EndReason;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _repository.UpdateAsync(marriage);

                await _lifeEventService.AddEventAsync(
                    marriage.Spouse1Id,
                    LifeEventType.Divorce,
                    marriage.EndDate.Value,
                    $"Развод с {GetFullName(spouse2.LastName, spouse2.FirstName, spouse2.MiddleName)}"
                );

                await _lifeEventService.AddEventAsync(
                    marriage.Spouse2Id,
                    LifeEventType.Divorce,
                    marriage.EndDate.Value,
                    $"Развод с {GetFullName(spouse1.LastName, spouse1.FirstName, spouse1.MiddleName)}"
                );

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Брак успешно расторгнут. " +
                    "Spouse1Id: {Spouse1Id}. " +
                    "Spouse2Id: {Spouse2Id}. " +
                    "LifeEventType: {LifeEventType}. " +
                    "EndDate: {EndDate}. " +
                    "Reason: {Reason}.",
                    spouse1.Id,
                    spouse2.Id,
                    LifeEventType.Divorce,
                    dto.DivorceDate,
                    dto.EndReason);

                return MarriageMapper.MapToMarriageDTO(marriage, spouse1, spouse2);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
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

        private async Task CheckSpouses(ShortPersonDTO spouse1, ShortPersonDTO spouse2, DateTime beginDate)
        {
            if (spouse1.Id.Equals(spouse2.Id))
                throw new InvalidMarriageDataException("Супруги не могут быть равны друг другу");

            await CheckSpouse(spouse1, beginDate);
            await CheckSpouse(spouse2, beginDate);
        }

        private async Task CheckSpouse(ShortPersonDTO spouse, DateTime beginDate)
        {
            if (spouse.DeathDate < beginDate)
                throw new InvalidMarriageDataException("Нельзя добавить брак умершему человеку");

            var married = await _repository.GetActiveMarriageAsync(spouse.Id);

            if (married != null)
                throw new ActiveMarriageExistsException();

            
        }

        private async Task CheckMarriage(Marriage marriage)
        {
            if (marriage.EndDate != null)
                throw new InvalidMarriageDataException("Брак уже закрыт");
        }

        private string GetFullName(string lastName, string firstName, string? middleName)
        {
            return string.Join(' ', new[] { lastName, firstName, middleName }
                .Where(s => !string.IsNullOrWhiteSpace(s)))
                .Trim();
        }
        #endregion
    }
}
