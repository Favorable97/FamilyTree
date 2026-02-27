namespace FamilyTree.Data.Interfaces
{
    public interface IMarriageRepository
    {
        /// <summary>
        /// Добавить брак
        /// </summary>
        /// <param name="marriage">Объект брака</param>
        /// <returns></returns>
        Task AddAsync(Marriage marriage);

        /// <summary>
        /// Изменить данные брака
        /// </summary>
        /// <param name="marriage">Объект брака</param>
        /// <returns></returns>
        Task UpdateAsync(Marriage marriage);

        /// <summary>
        /// Получить брак
        /// </summary>
        /// <param name="Id">Уникальный идентификатор брака</param>
        /// <returns></returns>
        Task<Marriage?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить все браки человека
        /// </summary>
        /// <param name="PersonId">Уникальный идентификатор человека</param>
        /// <returns></returns>
        Task<List<Marriage>> GetByPersonIdAsync(Guid personId);

        /// <summary>
        /// Получить текущий активный брак человека
        /// </summary>
        /// <param name="personId">Уникальный идентификатор человека</param>
        /// <returns></returns>
        Task<Marriage?> GetActiveMarriageAsync(Guid personId);
    }
}
