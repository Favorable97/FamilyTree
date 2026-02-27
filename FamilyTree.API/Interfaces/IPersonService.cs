using FamilyTree.API.Responses;
using FamilyTree.Data.Models;

namespace FamilyTree.API.Interfaces
{
    public interface IPersonService
    {
        /// <summary>
        /// Получение списка всех людей в системе
        /// </summary>
        /// <returns></returns>
        public Task<List<ShortPersonDTO>> GetAllPersonAsync();

        /// <summary>
        /// Получение человека по Id
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <returns></returns>
        public Task<PersonDTO> GetPersonByIdAsync(Guid id);

        /// <summary>
        /// Получение не полной информации о человеке по Id
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <returns></returns>
        public Task<ShortPersonDTO> GetShortPersonByIdAsync(Guid id);

        /// <summary>
        /// Сервис по добавлению человека
        /// </summary>
        /// <param name="requestAddPersonDTO">Объект данных о человеке</param>
        /// <returns></returns>
        public Task<PersonDTO> CreatePersonAsync(RequestAddPersonDTO requestAddPersonDTO);

        /// <summary>
        /// Сервис по изменению информации о человеке
        /// </summary>
        /// <param name="id">Id человека</param>
        /// <param name="requestUpdatePersonDTO">Информация для изменения</param>
        /// <returns></returns>
        public Task<PersonDTO> UpdatePersonAsync(Guid id, RequestUpdatePersonDTO requestUpdatePersonDTO);

        /// <summary>
        /// Удаление человека
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task DeletePersonAsync(Guid id);
    }
}
