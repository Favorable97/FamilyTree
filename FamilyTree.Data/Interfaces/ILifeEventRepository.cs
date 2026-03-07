using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Interfaces
{
    public interface ILifeEventRepository
    {
        /// <summary>
        /// Добавление жизненных событий человека
        /// </summary>
        /// <param name="lifeEvent">Объект события</param>
        /// <returns></returns>
        public Task AddEventAsync(LifeEvent lifeEvent);

        /// <summary>
        /// Получение жизненных событий человека
        /// </summary>
        /// <param name="personId">Уникальный идентификатор человека</param>
        /// <returns>Список жизненных событий человека</returns>
        public Task<List<LifeEvent>> GetByPersonIdAsync(Guid personId);
    }
}
