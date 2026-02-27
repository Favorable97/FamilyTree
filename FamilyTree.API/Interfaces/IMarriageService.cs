namespace FamilyTree.API.Interfaces
{
    public interface IMarriageService
    {
        /// <summary>
        /// Создание брака
        /// </summary>
        /// <param name="dto">Объект, содержащий информацию о браке</param>
        /// <returns>DTO для front-end</returns>
        Task<MarriageDTO> CreateMarriageAsync(RequestAddMarriageDTO dto);

        /// <summary>
        /// Развод
        /// </summary>
        /// <param name="dto">Объект, содержащий информацию о разводе</param>
        /// <returns>DTO для front-end</returns>
        Task<MarriageDTO?> DivorceAsync(RequestAddDivorceDTO dto);
        
        /// <summary>
        /// Получить текущего супруга
        /// </summary>
        /// <param name="personId">Уникальный идентификатор персоны</param>
        /// <returns></returns>
        Task<ShortPersonDTO?> GetCurrentSpouseAsync(Guid personId);

        /// <summary>
        /// Получить историю браков
        /// </summary>
        /// <param name="personId">Уникальный идентификатор персоны</param>
        /// <returns></returns>
        Task<List<MarriageDTO>> GetMarriageHistoryAsync(Guid personId);
    }
}
