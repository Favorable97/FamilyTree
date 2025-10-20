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
        
        public Task AddPersonAsync(Person person);
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
        /// Связь ребенка с родителями
        /// </summary>
        /// <param name="childId">Идентификатор ребенка</param>
        /// <param name="parentId">Идентификатор родителя</param>
        /// <param name="parentRelation">Тип связи</param>
        /// <returns></returns>
        
        public Task SetParentAsync(Guid childId, Guid parentId, ParentRelation parentRelation);
    }
}
