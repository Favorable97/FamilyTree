using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с людьми
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Получить всех людей без родителей
        /// </summary>
        /// <returns></returns>
        public Task<List<Person>> GetAllPersonAsync();

        /// <summary>
        /// Получить человека
        /// </summary>
        /// <param name="id">ID человека</param>
        /// <returns></returns>
        public Task<Person?> GetPersonByIdAsync(Guid id);
        /// <summary>
        /// Добавить нового человека
        /// </summary>
        /// <param name="person">Добавляемая информация о человеке</param>
        /// <returns></returns>
        
        public Task CreatePersonAsync(Person person);
        
        /// <summary>
        /// Обновление информации о человеке
        /// </summary>
        /// <param name="person">Информация о человеке</param>
        /// <returns></returns>
        public Task UpdatePersonAsync(Person person);
        
        /// <summary>
        /// Удаление человека по ID
        /// </summary>
        /// <param name="id">ID человека</param>
        /// <returns></returns>
        public Task DeletePersonAsync(Guid id);

        /// <summary>
        /// Проверка существования по ФИО и дате рождения для предотвращения дублей
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="dateBirthday"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(
            string lastName,
            string firstName,
            string? middleName,
            DateTime dateBirthday);

        /// <summary>
        /// Проверка существования по идентификатору.
        /// Метод используется в случае, если нужно только узнать, существует
        /// такой Id или нет
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> ExistsByIdAsync (Guid id);

        /// <summary>
        /// Является ли человек родителем
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> IsParentAsync(Guid id);

        /// <summary>
        /// Получить всех детей человека
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<List<Person>> GetChildrenAsync(Guid id);
    }
}
